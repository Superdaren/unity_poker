﻿﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using LitJson;
using Google.Protobuf;

namespace NetProto
{
    public class LoginHandle : NetHandle
    {
        public LoginHandle()
        {
            handlerMap.Add(Api.ENetMsgId.user_login_ack, loginAck);
        }

        #region request

        /**
         * 用户登录
         */
        public void loginReq()
        {
            UserLoginReq loginReq = new UserLoginReq();
            loginReq.UserId = UserManager.Instance().authModel.user_id;
            loginReq.UniqueId = "888";
            loginReq.Token = UserManager.Instance().authModel.token;
            loginReq.ConnectTo = PrefsUtil.GetString(PrefsUtil.ServiceId);
            loginReq.IsReconnect = 0;

            NetCore.Instance.Send(Api.ENetMsgId.user_login_req, loginReq);
        }

        /**
         * 用户重连
         */
        public void ReConnect()
        {
            //loginReq();
        }

        #endregion

        #region action
        /**
         * 用户登录回复
         */
        public object loginAck(byte[] data)
        {
            UserLoginAck loginAction = UserLoginAck.Parser.ParseFrom(data);

            PrefsUtil.Set(PrefsUtil.ServiceId, loginAction.ServiceId);

            Debug.Log("登录成功!");

            return loginAction;
        }
        #endregion

        #region httpRequest
        /**
         * 快速登录
         */
        public void loginQuick(string account, Action<Error, AuthModel> action) 
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
			dic.Add("mobile_number", account);
            dic.Add("password", account);
            dic.Add("from", "jjjj");
            HttpUtil.Http.Post(URLManager.loginQuickUrl).Form(dic).OnSuccess(result =>
            {
                if (result != null) {
                    AuthModel authModel =   JsonMapper.ToObject<AuthModel>(result);
                    UserManager.Instance().authModel = authModel;
					if (action != null)
						action(null, authModel);

                }
			}).OnFail(result =>
            {
                if (action != null)
                action(new Error(500, null), null);
            }).Go();
        }

        /**
         * 帐号密码登录
         */
        public void login(string account, string password, Action<Error, UserInfo> action)
		{
			Dictionary<string, object> dic = new Dictionary<string, object>();
			dic.Add("mobile_number", account);
			dic.Add("password", password);
			dic.Add("from", "jjjj");
            HttpUtil.Http.Post(URLManager.loginUrl).Form(dic).OnSuccess(result =>
			{
				if (result != null)
				{
					AuthModel authModel = JsonMapper.ToObject<AuthModel>(result);
                    if(authModel.ret == 1){
                        UserManager.Instance().authModel = authModel;
                        authSuccess(action);
                    } else {
                        if(action != null)
                        action(new Error(authModel.ret, authModel.msg), null);
                    }
				}
			}).OnFail(result =>
			{
                if (action != null)
				action(new Error(500, null), null);
            }).Go();
		}

        /**
         * 授权成功获取用户信息
         */
        public static void authSuccess(Action<Error, UserInfo> action) {
            HttpUtil.Http.Get(URLManager.getUserInfoUrl()).OnSuccess(result =>
			{
				if (result != null)
				{
                    UserInfoResult userResult = JsonMapper.ToObject<UserInfoResult>(result);
                    if(userResult.ret == 1) {
                        UserManager.Instance().isLogin = true;
                        UserManager.Instance().userInfo = userResult.data;
                        // 用户数据持久化
                        UserManager.saveUserAuthToUserDefault(UserManager.Instance().authModel);
                        UserManager.saveUserInfoToUserDefault(UserManager.Instance().userInfo);
                        if (action != null)
                        action(null, userResult.data);
                    } else {
                        if (action != null)
                        action(new Error(userResult.ret, userResult.msg), null);
                    }
				}
			}).OnFail(result =>
			{
                if (action != null)
				action(new Error(500, null), null);
            }).Go();
        }

		/**
         * 获取token信息
         */
        public static void checkUserInfoToken(Action<Error, Dictionary<string, object>> action)
		{
			Dictionary<string, object> dic = new Dictionary<string, object>();
			dic.Add("unique_id", "33333");
            dic.Add("ver", 1);
			dic.Add("from", "jjjj");
            dic.Add("token", UserManager.getUserAuthFromUserDefault().token);
            dic.Add("user_id", UserManager.getUserAuthFromUserDefault().user_id);
            HttpUtil.Http.Post(URLManager.checkUserInfoToken).Form(dic).OnSuccess(result =>
			{
				if (result != null)
                {
                    Dictionary <string, object> tokenResult = JsonMapper.ToObject<Dictionary<string, object>>(result);
					if (action != null)
						action(null, tokenResult);
				}
			}).OnFail(result =>
			{
				if (action != null)
					action(new Error(500, null), null);
            }).Go();
		}

        /**
         * 刷新token信息
         */
        public static void refreshUserInfoToken(Action<Error> action)
		{
			Dictionary<string, object> dic = new Dictionary<string, object>();
			dic.Add("unique_id", "33333");
			dic.Add("refresh_token", UserManager.getUserAuthFromUserDefault().refresh_token);
			dic.Add("from", "jjjj");
			dic.Add("token", UserManager.getUserAuthFromUserDefault().token);
			dic.Add("user_id", UserManager.getUserAuthFromUserDefault().user_id);
            HttpUtil.Http.Post(URLManager.refreshUserInfoToken).Form(dic).OnSuccess(result =>
			{
				if (result != null)
				{
					AuthModel authModel = JsonMapper.ToObject<AuthModel>(result);
                    if(authModel.ret == 1){
                        UserManager.Instance().isLogin = true;
						UserManager.Instance().authModel = authModel;
                        UserManager.Instance().userInfo = UserManager.getUserInfoFromUserDefault();
						// 更新本地的token信息
						UserManager.saveUserAuthToUserDefault(UserManager.Instance().authModel);
						if (action != null)
							action(null);
                    } else {
						if (action != null)
							action(new Error(authModel.ret, null));
                    }
				}
			}).OnFail(result =>
			{
				if (action != null)
					action(new Error(500, null));
            }).Go();
		}

        #endregion
    }
}

