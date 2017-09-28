using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;
using LitJson;

public class ShopingController : MonoBehaviour
{
    // 钻石商品物体集合
    List<GameObject> goldsObjectList;

	// Use this for initialization
	void Start()
	{
        // add purchaseToIOS component
        gameObject.AddComponent<PurchaseToIOS>();
        
        ShoppingHandle.getChargeGoodsList((error, resultList) =>
		{
			if (error == null)
			{
                MainData.Instance().chargeGoods = resultList.list;
				loadGoldView();
			}
		});
	}

    void loadGoldView() {
		GameObject commonUIPrefab = Resources.Load("Prefabs/GoldsItem") as GameObject;
        goldsObjectList = new List<GameObject>();
        for (int i = 0; i < MainData.Instance().chargeGoods.Length; i++)
        {
            ChargeGoodsInfo chargeGoods = MainData.Instance().chargeGoods[i];
			GameObject golds = Instantiate(commonUIPrefab) as GameObject;
            float width = golds.GetComponent<RectTransform>().sizeDelta.x;
			golds.transform.parent = GameObject.Find("GoldContent/Viewport/Content").transform;
            golds.name = "GoldsItem" + i;
            golds.transform.localPosition = new Vector3(300 + i % 3 * (width + 150), -300 - (i / 3 * 400), 0);
			golds.transform.localScale = new Vector3(1, 1, 0);

            //Image GoldTypeImage = golds.Find<Image>("GoldContent/Viewport/Content/"+golds.name +"/TopImage/GoldTypeImage");
            Text MondyText = golds.Find<Text>("GoldContent/Viewport/Content/"+golds.name +"/TopImage/MondyText");
            Text ConversionText = golds.Find<Text>("GoldContent/Viewport/Content/"+golds.name +"/TopImage/ConversionText");
            Text DiamondText = golds.Find<Text>("GoldContent/Viewport/Content/"+golds.name +"/DiamondImage/DiamondText");

            //GoldTypeImage.SetWebImage(chargeGoods.image, "");
            MondyText.text = chargeGoods.price + "万";
            ConversionText.text = chargeGoods.goods_describe;
            DiamondText.text = chargeGoods.price.ToString();

            golds.GetComponent<Button>().onClick.AddListener(() => {
                if(UserManager.Instance().userInfo.diamond_balance < chargeGoods.price) {
                    PopUtil.ShowMessageBoxWithConfirm("提示", "钻石不够啦,赶紧去买!");
                    return;
                }
                ShoppingHandle.exchargeDiamond(chargeGoods.id.ToString(),(error) =>
			    {
					if (error == null)
					{
                        PopUtil.ShowMessageBoxWithConfirm("提示", "兑换成功!");
                        UserInfoRefreshManager.refreshUserInfo(null);
                    } else {
                        PopUtil.ShowMessageBoxWithConfirm("提示", "兑换失败");
                    }
			    });
            });

            goldsObjectList.Add(golds);
        }
    }

    /**
     * ========================点击事件=============================
     */
    public void closeBtnClick() {
        gameObject.SetActive(false);
    }
}
