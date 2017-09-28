using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System;
using LitJson;

/**
 * 内购状态
 */
public enum PurchaseProductStatus {
	PKPurchaseProductStatusPurchased                    = 1,          // 购买完成
	PKPurchaseProductStatusRestored                     = 2,          // 恢复购买成功
	PKPurchaseProductStatusPurchasing                   = 3,          // 正在请求付费信息

	PKPurchaseProductStatusFailed                       = 4,          // 购买失败
	PKPurchaseProductStatusCancelled                    = 5,          // 用户取消
	PKPurchaseProductStatusNotAllowed                   = 6,          // 设备不允许内购
	PKPurchaseProductStatusPermissionDenied             = 7,          // 用户不允许内购

	PKPurchaseProductStatusRequestingPurchase           = 8,          // 正在请求购买信息
	PKPurchaseProductStatusRequestPurchaseSuccess       = 9,          // 查询产品信息成功
	PKPurchaseProductStatusRequestPurchaseFailed        = 10,         // 查询产品信息失败
}

public class PurchaseToIOS : MonoBehaviour
{
    // 用户凭证持久化key
    static string userPurchaseReceiptKey = "userPurchaseReceiptKey";    

	[DllImport("__Internal")]
	static extern void _purchaseProductWithID(string name, string productID);

    /**
     * 点击购买商品
     */
    public static void purchaseProductWithID(string name, string productID) {
        if(productID == null || productID == ""){
            return;
        }

        _purchaseProductWithID(name, productID);
    }

    /**
     * 购买信息回调
     */
    public void purchaseProductCallBack(string msg){
        // 获取iOS传过来的消息
		Dictionary<string, string> dicMsg = UnityIOSAndroid.parseMsg(msg);
        string purchaseStatus = dicMsg["purchaseStatus"];
        string UUID = dicMsg["UUID"];
        string productID = dicMsg["productID"];

        PurchaseProductStatus status = (PurchaseProductStatus)Enum.Parse(typeof(PurchaseProductStatus), purchaseStatus);

        switch(status) {
            case PurchaseProductStatus.PKPurchaseProductStatusPurchased:
                string receipt = dicMsg["receipt"];
                checkReceiptIsValid(productID, receipt, UUID, gameObject);
                break;
			case PurchaseProductStatus.PKPurchaseProductStatusRestored:
                PopUtil.ShowMessageBoxWithConfirm("提示", "恢复购买成功!");
				break;
			case PurchaseProductStatus.PKPurchaseProductStatusPurchasing:
                //CommonUI.showLoadingView(gameObject, "正在获取支付信息");
				break;
			case PurchaseProductStatus.PKPurchaseProductStatusFailed:
                PopUtil.ShowMessageBoxWithConfirm("提示", "购买失败!");
				break;
			case PurchaseProductStatus.PKPurchaseProductStatusCancelled:
                PopUtil.ShowMessageBoxWithConfirm("提示", "用户取消购买!");
				break;
			case PurchaseProductStatus.PKPurchaseProductStatusNotAllowed:
                PopUtil.ShowMessageBoxWithConfirm("提示", "该设备不支持内购");
				break;
			case PurchaseProductStatus.PKPurchaseProductStatusPermissionDenied:
                PopUtil.ShowMessageBoxWithConfirm("提示", "用户不支持内购");
				break;
			case PurchaseProductStatus.PKPurchaseProductStatusRequestingPurchase:
                PopUtil.ShowLoadingView("Loading....");
				break;
			case PurchaseProductStatus.PKPurchaseProductStatusRequestPurchaseSuccess:
                //CommonUI.showLoadingView(gameObject, " 商品信息获取成功!");
				break;
			case PurchaseProductStatus.PKPurchaseProductStatusRequestPurchaseFailed:
                PopUtil.ShowMessageBoxWithConfirm("提示", "拉取商品信息失败,请重试!");
				break;
        }
    }

    /**
     * 检查凭证是否有效
     */
    public static void checkReceiptIsValid(string productID, string receipt, string uuid, GameObject gameObject)
	{
        Dictionary<string, object> receiptDict = new Dictionary<string, object>();
		receiptDict.Add("apple_data", receipt);
        receiptDict.Add("user_id", UserManager.Instance().authModel.user_id);
        receiptDict.Add("charge_goods_id", productID);
        receiptDict.Add("price", MainData.Instance().getGoodsPrice(productID));
        receiptDict.Add("uuid", uuid);

		// 发送之前先des加密
        string RSAString = DESBase64.DesEncryptWithKey(JsonMapper.ToJson(receiptDict), DESBase64.purchaseKey);
		Dictionary<string, object> RSADic = new Dictionary<string, object>();
		RSADic.Add("data", RSAString);

        HttpUtil.Http.Post(URLManager.appStoreCheckPurchaseCertUrl).Form(RSADic).OnSuccess(result =>
		{
            Debug.Log("OnSuccess: result------------------" + result);
            Dictionary<string, object> resultDict = JsonMapper.ToObject<Dictionary<string, object>>(result);
            string code = resultDict["code"].ToString();
            int codeInt = int.Parse(code);
            if (codeInt == 1) { // 凭证验证成功
                removeReceipt(uuid);
                // 刷新主界面的信息
                UserInfoRefreshManager.refreshUserInfo(null);
				PopUtil.ShowMessageBoxWithConfirm("提示", "购买成功!");
            } else if(codeInt == -1) { //  服务器凭证验证成功但是数据操作异常,需要重新发送一次
                PopUtil.ShowMessageBoxWithConfirmAndCancle("提示", "购买失败!点击确定重试!", () =>
                { // 点击重新发送服务器验证
                    checkReceiptIsValid(productID, receipt, uuid, gameObject);
                }, () => {  // 取消存储在本地,下次启动时候发送
                    saveReceipt(receiptDict, uuid);
                });
            } else if(codeInt == 0) { // 凭证无效
                removeReceipt(uuid);
                PopUtil.ShowMessageBoxWithConfirm("提示", "购买失败!");
            }
		}).OnFail(result =>
		{
            Debug.Log("OnFail: result------------------"+ result);
            // 网络问题
			PopUtil.ShowMessageBoxWithConfirmAndCancle("提示", "购买失败!点击确定重试!", () =>
			{ // 点击重新发送服务器验证
				checkReceiptIsValid(productID, receipt, uuid, gameObject);
			}, () =>
			{  // 取消存储在本地,下次启动时候发送
				saveReceipt(receiptDict, uuid);
			});
		}).GoSync();
	}

    /**
     * 移除凭证
     */
    public static void removeReceipt(string uuid) {
		List<Dictionary<string, object>> receiptList = getReceiptList();
        List<Dictionary<string, object>> receiptListTem = new List<Dictionary<string, object>> { };
        foreach(Dictionary<string, object>dic in receiptList){
            if(dic["uuid"].ToString() == uuid) {
                continue;
            }

            receiptListTem.Add(dic);
        }
        if(receiptListTem.Count != 0){
            PlayerPrefs.SetString(userPurchaseReceiptKey, JsonMapper.ToJson(receiptList));
        } else {
            PlayerPrefs.SetString(userPurchaseReceiptKey, "");
        }
    }

    /**
     * 保存凭证本地持久化
     */
    public static void saveReceipt(Dictionary<string, object> receiptDic, string uuid) {
        
        List<Dictionary<string, object>> receiptList = getReceiptList();

        bool isSame = false;
		foreach (Dictionary<string, object> dic in receiptList)
		{
            if(dic["uuid"].ToString() == uuid) {
                isSame = true;
            }
		}

        if (isSame) return; // 已经存在不需要再次保存

		Dictionary<string, object> saveDict = new Dictionary<string, object>();
		saveDict.Add("uuid", uuid);
        saveDict.Add("receipt", JsonMapper.ToJson(receiptDic));
        receiptList.Add(saveDict);

		string receiptStr = JsonMapper.ToJson(receiptList);
		// 保存之前先des加密
		string desReceiptStr = DESBase64.DesEncrypt(receiptStr);

        PlayerPrefs.SetString(userPurchaseReceiptKey, desReceiptStr);
    }

    /**
     * 获取本地持久化凭证列表
     */
    public static List<Dictionary<string, object>> getReceiptList() {
		string enReceiptStr = PlayerPrefs.GetString(userPurchaseReceiptKey);
		List<Dictionary<string, object>> receiptList = new List<Dictionary<string, object>> { };
		if (enReceiptStr != null && enReceiptStr != "")
		{
			// 取出来先des解密
			string deReceiptStr = DESBase64.DesDecrypt(enReceiptStr);
			receiptList = JsonMapper.ToObject<List<Dictionary<string, object>>>(deReceiptStr);
		}

        return receiptList;
    }

    /**
     * 重新发送持久化的appStore凭证
     */
    public static void resendAppStoreRequestReceipt() {

        List<Dictionary<string, object>> receiptList = getReceiptList();
        if (receiptList.Count == 0) return;

		foreach (Dictionary<string, object> dic in receiptList)
		{
			// 发送之前先des加密
			string RSAString = DESBase64.DesDecryptWithKey(dic["receipt"].ToString(), DESBase64.purchaseKey);
			Dictionary<string, object> RSADic = new Dictionary<string, object>();
			RSADic.Add("data", RSAString);

			HttpUtil.Http.Post(URLManager.appStoreCheckPurchaseCertUrl).Form(RSADic).OnSuccess(result =>
			{
				Dictionary<string, object> resultDict = JsonMapper.ToObject<Dictionary<string, object>>(result);
				string code = resultDict["code"].ToString();
				int codeInt = int.Parse(code);
				if (codeInt == 1) { // 凭证验证成功
                    removeReceipt(dic["uuid"].ToString());
				}
				else if (codeInt == -1) { //  服务器凭证验证成功但是数据操作异常,需要重新发送一次
				    //resendAppStoreRequestReceipt();
				}
				else if (codeInt == 0) { // 凭证无效
				    removeReceipt(dic["uuid"].ToString());
				}
			}).OnFail(result =>
			{
				Debug.Log("appStoreCheckPurchase------fail:" + result);
            }).GoSync();
		}
    }
}
