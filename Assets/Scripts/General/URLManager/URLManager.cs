using UnityEngine;
using System.Collections;

public class URLManager
{
    // 是否是测试模式
    public static bool isDebug = true;

    #region base
    public static string apiBaseUrl = isDebug ? "http://192.168.60.164:10076/" : "";
    #endregion

    #region login
    // 快速登录
    public static string loginQuickUrl = apiBaseUrl + "auth/login/quick";
    // 手机帐号密码登录
    public static string loginUrl = apiBaseUrl + "auth/login";
    // 手机号注册
    public static string registerMobileUrl = apiBaseUrl + "auth/register/mobile";
    // 第三方登录
    public static string thirdLoginUrl = apiBaseUrl + "auth/login/tp";
    // 获取用户信息
    public static string getUserInfoUrl()
    {
        return apiBaseUrl + "user/" + UserManager.Instance().authModel.user_id + "/info?token=" + UserManager.Instance().authModel.token;
    }
    // 获取token信息
    public static string checkUserInfoToken = apiBaseUrl + "auth/token/info";
    // 刷新token信息
    public static string refreshUserInfoToken = apiBaseUrl + "auth/token/refresh";
    // 购买钻石
    public static string appStoreCheckPurchaseCertUrl = "http://test-pay-chess.yyjinbao.com/applepay";
    //public static string appStoreCheckPurchaseCertUrl = "http://lumen.chess.com/applepay";
    #endregion

    #region main
    // 房间列表
    public static string roomListUrl = apiBaseUrl + "room/list";
    // 钻石列表
    public static string diamondListUrl = apiBaseUrl + "charge/charge_goods/list";
    // 商品列表
    public static string chargeGoodsListUrl = apiBaseUrl + "goods/list";
    // 任务列表
    public static string taskListUrl()
    {
        return apiBaseUrl + "task/" + UserManager.Instance().authModel.user_id + "/list?token=" + UserManager.Instance().authModel.token;
    }
    // 领取任务奖励
    public static string taskReceiveUrls(string taskId)
	{
		return apiBaseUrl + "task/" + UserManager.Instance().authModel.user_id + "/receive?token=" + UserManager.Instance().authModel.token + "&task_id=" + taskId;
	}
	// 背包列表
	public static string bagList()
	{
		return apiBaseUrl + "user/" + UserManager.Instance().authModel.user_id + "/bag/list?token=" + UserManager.Instance().authModel.token;
	}
	#endregion

    #region user
    // 用户详细信息
    public static string userDetail()
    {
        return apiBaseUrl + "/user/" + UserManager.Instance().authModel.user_id + "/detail?token=" + UserManager.Instance().authModel.token;
    }

    // 刷新用户信息
    public static string refreshUserInfoUrl()
    {
        return apiBaseUrl + "/user/" + UserManager.Instance().authModel.user_id + "/info?token=" + UserManager.Instance().authModel.token;
    }
    // 钻石兑换
    public static string diamondExchangeUrl(string goodID)
    {
        return apiBaseUrl + "/user/" + UserManager.Instance().authModel.user_id + "/exchange?token=" + UserManager.Instance().authModel.token
               + "&goods_id=" + goodID;
    }
    public static string goodsListUrl = apiBaseUrl + "charge/charge_goods/list";

    // 更新昵称
    public static string postNickname()
    {
        return apiBaseUrl + "user/" + UserManager.Instance().authModel.user_id + "/profile/nickname?token=" + UserManager.Instance().authModel.token;
    }
    // 签到列表
    public static string signInListUrl()
    {
        return apiBaseUrl + "user/" + UserManager.Instance().authModel.user_id + "/checkin/list?token=" + UserManager.Instance().authModel.token;
    }
    // 领取签到奖励
    public static string receiveSignInRewardUrl(int isMore)
    {
        return apiBaseUrl + "user/" + UserManager.Instance().authModel.user_id + "/checkin?token=" + UserManager.Instance().authModel.token + "&is_more=" + isMore;
    }
    // 退出登录
    public static string logoutUrl()
    {
        return apiBaseUrl + "/user/" + UserManager.Instance().authModel.user_id + "/logout?token=" + UserManager.Instance().authModel.token;
    }
    #endregion

    #region room

    // 获取上局牌局数据
    public static string lastGameUrl(string tableId)
    {
        return apiBaseUrl + "game/" + UserManager.Instance().authModel.user_id + "/last_game?token=" + UserManager.Instance().authModel.token + "&table_id=" + tableId;
    }
	//recordList
	// 获取上局牌局数据
	public static string recordList(string page_size, string page_num)
	{
		return apiBaseUrl + "game/" + UserManager.Instance().authModel.user_id + "/game_list?token=" + UserManager.Instance().authModel.token + "&page_size=" + page_size + "&page_num=" + page_num;
	}
    #endregion
}
