using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System;
using LitJson;
using NetProto;

public class SDKToIOS : Singleton<SDKToIOS>
{
    // 用于微信,QQ登录的回调
    Action<Error, UserInfo> _action;

   #region  common handle
    // 初始化SDK
   public void InitSDK()
	{
        /** 微信 **/
		_weixinInit(IOSConfig.kWeiXinAppID, IOSConfig.kWeiXinSecret);
		/** QQ **/
        _qqInit(IOSConfig.kQQID, IOSConfig.kQQKey);
	}

    /**
     * 处理获取用户返回来的消息
     */
    void handleGetUserInfo(string result) {
		if (result != null)
		{
			AuthModel authModel = JsonMapper.ToObject<AuthModel>(result);
			if (authModel.ret == 1)
			{
				UserManager.Instance().authModel = authModel;
				LoginHandle.authSuccess(_action);
			}
			else
			{
				if (_action != null)
					_action(new Error(authModel.ret, authModel.msg), null);
			}
		}
    }

	#endregion


	#region weixin SDK
	[DllImport("__Internal")]
    static extern void _weixinInit(string weiXinAppID, string weiXinSecret);

    [DllImport("__Internal")]
    static extern void _weixinLogin(string name);

    /**
     * 点击微信登录
     */
    public void weixinLogin(string name, Action<Error, UserInfo> action) {
        _action = action;

        _weixinLogin(name);
    }

    /**
     * 微信登录返回结果
     */
    public void weixinLoginCallBack(string msg) {
        Dictionary<string, string> dicMsg = UnityIOSAndroid.parseMsg(msg);

		Dictionary<string, object> dic = new Dictionary<string, object>();
		dic.Add("type", 3);
		dic.Add("key", dicMsg["openID"]);
		dic.Add("from", "jjjj");
		dic.Add("unique_id", "UniqueId");
		dic.Add("channel", "jjjj");
        dic.Add("binding_param", "");

        PopUtil.ShowLoadingView("登录中...");

        HttpUtil.Http.Post(URLManager.thirdLoginUrl).Form(dic).OnSuccess(result =>
		{
            handleGetUserInfo(result);
		}).OnFail(result =>
		{
			if (_action != null)
				_action(new Error(500, null), null);
		}).GoSync();
    }
	#endregion

	#region QQ SDK
	[DllImport("__Internal")]
    static extern void _qqInit(string QQID, string kQQKey);

	[DllImport("__Internal")]
	static extern void _qqLogin(string name);

	/**
     * 点击QQ登录
     */
	public void qqLogin(string name, Action<Error, UserInfo> action)
	{
		_qqLogin(name);

        _action = action;
	}

	/**
     * QQ登录返回结果
     */
	public void qqLoginCallBack(string msg)
	{
		Dictionary<string, string> dicMsg = UnityIOSAndroid.parseMsg(msg);

		Dictionary<string, object> dic = new Dictionary<string, object>();
		dic.Add("type", 1);
		dic.Add("key", dicMsg["accessToken"]);
		dic.Add("from", "jjjj");
		dic.Add("unique_id", "UniqueId");
		dic.Add("channel", "jjjj");
		dic.Add("binding_param", "");

        PopUtil.ShowLoadingView("登录中...");

        HttpUtil.Http.Post(URLManager.thirdLoginUrl).Form(dic).OnSuccess(result =>
		{
            Debug.Log("qqLoginCallBack---" + result);
			handleGetUserInfo(result);
		}).OnFail(result =>
		{
            Debug.Log("qqLoginCallBack---" + result);
			if (_action != null)
				_action(new Error(500, null), null);
		}).GoSync();
	}
    #endregion
}
