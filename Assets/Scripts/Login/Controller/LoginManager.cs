﻿﻿﻿﻿﻿﻿﻿﻿﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NetProto;
using System;
using DG.Tweening;

public class LoginManager : MonoBehaviour
{

    public InputField countField;
    public InputField passwordField;
    public GameObject registView;

    public GameObject login;

    LoginHandle loginHandle;

    public void onLoginBtnClick()
    {
        string count = countField.text;
        string password = passwordField.text;

        if (count == null || count == "")
        {
            PopUtil.ShowTotoast("帐号不能为空");
            return;
        }

        if (password == null || password == "")
        {
            PopUtil.ShowTotoast("密码不能为空");
            return;
        }

        PopUtil.ShowLoadingView("登录中...");
        loginHandle.login(count, password, (error, result) =>
        {
            handleLoginResult(error, result);
        });
    }

    public void registBtnClick()
    {
        registView.GetComponent<Transform>().localScale = new Vector3(0.5f, 0.5f, 0.5f);
        registView.SetActive(true);

        registView.transform.DOScale(new Vector3(1, 1, 1), 0.1f);
    }

    /**
     * 微信登录
     */
    public void weixinBtnClick()
    {
#if UNITY_IOS
        SDKToIOS.Instance.weixinLogin(gameObject.name, (error, result) =>
        {
            handleLoginResult(error, result);
        });
#elif UNITY_ANDROID
        print("click  WechatLogin");
        SDKToAndroid.Instance.WechatLogin((error, result) =>
        {
            handleLoginResult(error, result);
        });
#endif
    }

    /**
     * QQ登录
     */
    public void qqBtnClick()
    {
#if UNITY_IOS
        SDKToIOS.Instance.qqLogin(gameObject.name, (error, result) =>
        {
            handleLoginResult(error, result);
        });
#elif UNITY_ANDROID
        print("click  QQLogin");
        SDKToAndroid.Instance.QQLogin((error, result) =>
        {
            handleLoginResult(error, result);
        });
#endif
    }
    // Android第三方登录回调Unity
    public void LoginComplete(string result)
    {
        print("click  LoginComplete");
        SDKToAndroid.Instance.LoginCallBack(result);
    }

    // 登录结果回调
    public void handleLoginResult(Error error, UserInfo userInfo)
    {
        if (error == null)
        {
            NetCore.Instance.RegisterAction(NetProto.Api.ENetMsgId.user_login_ack, (loginAction) =>
            {
                UserLoginAck userLoginAck = (UserLoginAck)loginAction;
                Debug.Log("loginAction-----" + userLoginAck.BaseAck.Ret);
                if (userLoginAck.BaseAck.Ret == 1)
                {
                    Application.LoadLevel("main_yg");
                }
                else
                {
                    isLoadingView(false);
                }
            });
            loginHandle.loginReq();
        }
        else
        {
            isLoadingView(false);
            if (error.code == (int)Error.ErrorCode.Error)
            {
                PopUtil.ShowTotoast("网络有问题,或者联系: jX");
            }
            else if (error.code == (int)Error.ErrorCode.Cancel)
            {
                PopUtil.ShowTotoast("已取消登录");
            }
            else
            {
                PopUtil.ShowTotoast(error.msg);
            }
        }
    }


    // Use this for initialization
    void Start()
    {
        //PrefsUtil.DeleteAll();

        // 显示加载界面
        isLoadingView(true);

		// 将第三方SDKToIOS添加到LoginManager上面
		gameObject.AddComponent<SDKToIOS>();

        // 注册loginHandle
        registHandle();

        checkUserInfoToken();
    }

    /**
     * 是否是正在加载的界面
     */
    public void isLoadingView(bool flag)
    {
        string loadingText = "登录中...";
        if (UserManager.getUserAuthFromUserDefault() == null)
        {
            loadingText = "加载中...";
        }
        if (flag) {
            PopUtil.ShowLoadingView(loadingText);
        } else {
            PopUtil.DismissLoadingView();
        }
        login.SetActive(!flag);
    }

    /**
     * 注册loginHandle
     */
    void registHandle()
    {
        loginHandle = new LoginHandle();
        NetCore.Instance.registHandle(loginHandle);

        registView.GetComponent<RegistManager>().loginHandle = loginHandle;
    }

    /**
     * 检查用户的token
     */
    void checkUserInfoToken()
    {
        if (UserManager.getUserAuthFromUserDefault() == null)
        {
            isLoadingView(false);
            return;
        }
        LoginHandle.checkUserInfoToken((error, tokenResult) =>
        {
            if (error == null)
            {
                string ret = tokenResult["ret"].ToString();
                UserAuthTokenStatus status = (UserAuthTokenStatus)Enum.Parse(typeof(UserAuthTokenStatus), ret);
                // 需要重新登录
                if (status == UserAuthTokenStatus.AuthALreadyLogin || status == UserAuthTokenStatus.AuthExpire || status == UserAuthTokenStatus.AuthKickedOut)
                {
                    isLoadingView(false);
                }
                else if (status == UserAuthTokenStatus.AuthNoReToken) // 不刷新token,直接登录
                {
                    UserManager.Instance().isLogin = true;
                    UserManager.Instance().authModel = UserManager.getUserAuthFromUserDefault();
                    UserManager.Instance().userInfo = UserManager.getUserInfoFromUserDefault();
                    handleLoginResult(null, UserManager.Instance().userInfo);
                }
                else if (status == UserAuthTokenStatus.AuthReToken) // 刷新token
                {
                    LoginHandle.refreshUserInfoToken((refreshError) =>
                    {
                        if (refreshError == null)
                        {
                            handleLoginResult(null, UserManager.Instance().userInfo);
                        }
                        else
                        {
                            isLoadingView(false);
                        }
                    });
                }
                else
                {
                    isLoadingView(false);
                }
            }
            else
            {
                isLoadingView(false);
            }
        });
    }
}
