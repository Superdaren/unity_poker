using System;
using UnityEngine;
using LitJson;
using NetProto;
using System.Collections.Generic;

public class SDKToAndroid : Singleton<SDKToAndroid>
{
    public AndroidJavaClass jc;
    public AndroidJavaObject jo;

    // 用于微信,QQ登录的回调
    Action<Error, UserInfo> _action;

    /**
     * 初始化SDK
     */
    public void InitSDK()
    {
        jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        jo.Call("InitSDK", AndroidConfig.QQ_APPID, AndroidConfig.WECHAT_APPID, AndroidConfig.WEIBO_APPID);
    }

    /**
     * QQ登录
     */
    public void QQLogin(Action<Error, UserInfo> action)
    {
        _action = action;
        jo.Call("QQLogin");
    }

    /**
     * 微信登录
     */
    public void WechatLogin(Action<Error, UserInfo> action)
    {
        _action = action;
        jo.Call("WechatLogin");
    }

    /**
     * 微博登录
     */
    public void WeiboLogin(Action<Error, UserInfo> action)
    {
        _action = action;
        jo.Call("WeiboLogin");
    }

    /**
     * 登录回调
     */
    public void LoginCallBack(string param)
    {
        Dictionary<string, string> dicMsg = UnityIOSAndroid.parseMsg(param);
        if (Int32.Parse(dicMsg["ret"]) == 1)
        {
            HttpUtil.Http.Post(URLManager.thirdLoginUrl).Form(dicMsg).OnSuccess(result =>
            {
                handleGetUserInfo(result);
            }).OnFail(result =>
            {
                _action(new Error((int)Error.ErrorCode.Error, null), null);
            }).GoSync();
        }
        else if(Int32.Parse(dicMsg["ret"]) == 0)
        {
            _action(new Error((int)Error.ErrorCode.Cancel, null), null);
        }
        else
        {
            _action(new Error((int)Error.ErrorCode.Error, null), null);
        }
    }

    /**
     * 处理获取用户返回来的消息
     */
    public void handleGetUserInfo(string result)
    {
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

}
