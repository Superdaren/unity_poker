using UnityEngine;
using System.Collections;
using LitJson;

public enum UserAuthTokenStatus
{
	AuthFailedStatus    = -998,         //token错误
	AuthALreadyLogin    = -997,         //token已刷新
	AuthExpire          = -996,         //token过期
	AuthKickedOut       = -995,         //踢出
	AuthReToken         = -994,         //应刷新
    AuthNoReToken       = 1,            //无需刷新
}

public class UserManager
{
	static string userInfoUserDefaultKey = "userInfoUserDefaultKey";
	static string userAuthUserDefaultKey = "userAuthUserDefaultKey";

	static UserManager userManager = null;

	public static UserManager Instance()
	{
		if (userManager == null)
		{
			userManager = new UserManager();
		}
		return userManager;
	}

    // 是否登录
    public bool isLogin;
    // 授权相关的模型
    public AuthModel authModel;
    // 用户信息
    public UserInfo userInfo;

    /**
     * 存储用户信息
     */
    public static void saveUserInfoToUserDefault(UserInfo userInfo) 
    {
        if (userInfo == null) return;

        string userInfoStr = JsonMapper.ToJson(userInfo);
        // 对用户信息进行DESBase64 加密
        string userInfoEncStr = DESBase64.DesEncrypt(userInfoStr);
        PlayerPrefs.SetString(userInfoUserDefaultKey, userInfoEncStr);
    }

	/**
     * 获取用户信息
     */
	public static UserInfo getUserInfoFromUserDefault()
	{
        string userInfoStr = PlayerPrefs.GetString(userInfoUserDefaultKey);
        if(userInfoStr == null ||userInfoStr == "") return null;

        string userInfoDesStr = DESBase64.DesDecrypt(userInfoStr);
        UserInfo userInfo = JsonMapper.ToObject<UserInfo>(userInfoDesStr);

        return userInfo;
	}

	/**
     * 存储用户授权信息
     */
    public static void saveUserAuthToUserDefault(AuthModel authModel)
	{
        if (authModel == null) return;

		string authModelStr = JsonMapper.ToJson(authModel);
		// 对用户信息进行DESBase64 加密
		string authModelEncStr = DESBase64.DesEncrypt(authModelStr);
        PlayerPrefs.SetString(userAuthUserDefaultKey, authModelEncStr);
	}

	/**
     * 获取用户授权信息
     */
    public static AuthModel getUserAuthFromUserDefault()
	{
		string authModelStr = PlayerPrefs.GetString(userAuthUserDefaultKey);
        if (authModelStr == null || authModelStr == "") return null;

		string authModelDesStr = DESBase64.DesDecrypt(authModelStr);
        AuthModel authModel = JsonMapper.ToObject<AuthModel>(authModelDesStr);

		return authModel;
	}

	/**
     * 用户退出登录
     */
	public static void logout()
	{
		PlayerPrefs.DeleteAll();
	}
}
