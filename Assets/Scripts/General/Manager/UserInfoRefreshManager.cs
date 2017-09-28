using UnityEngine;
using System.Collections;
using LitJson;
using UnityEngine.UI;
using System;

public class UserInfoRefreshManager
{
	/**
     * 刷新用户信息
     */
    public static void refreshUserInfo(Action<Error> action)
	{
        HttpUtil.Http.Get(URLManager.refreshUserInfoUrl()).OnSuccess(result =>
		{
			if (result != null)
			{
                UserInfoRefresh goodsList = JsonMapper.ToObject<UserInfoRefresh>(result);
                UserInfo userInfo = goodsList.data;

                // 更新本地信息
                UserManager.Instance().userInfo = userInfo;
                UserManager.saveUserInfoToUserDefault(userInfo);

				if (action != null)
                    action(null);

                refreshMianView();
            } else {
				if (action != null)
					action(new Error(500, null));
            }
        }).OnFail(result => {
            if (action != null)
                action(new Error(500, null));
        }).GoSync();
	}

    /**
     * 更新主界面的信息
     */
    static void refreshMianView() {
        GameObject userInfoView =  GameObject.Find("UserInfoView");
		Text userName = userInfoView.Find<Text>(userInfoView.name + "/UserNameBackground/UserName");
		Text goldText = userInfoView.Find<Text>(userInfoView.name + "/UserGoldBtn/Gold");
		Text diamondText = userInfoView.Find<Text>(userInfoView.name + "/UserdiamondBtn/Diamond");
		//Image userIcon  = userInfoView.Find<Image>(userInfoView.name + "/UserIcon");

		userName.text = UserManager.Instance().userInfo.nick_name;
		goldText.text = UserManager.Instance().userInfo.balance.ToString();
		diamondText.text = UserManager.Instance().userInfo.diamond_balance.ToString();
    }
}
