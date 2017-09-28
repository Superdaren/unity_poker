using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using LitJson;

namespace NetProto
{
    public class RegistHandle : NetHandle
    {
        public void registAction(string account, string password, Action<Error, UserInfo> action)
		{
	        Dictionary<string, object> dic = new Dictionary<string, object>();
	        dic.Add("mobile_number", account);
	        dic.Add("password", password);
	        dic.Add("from", "ios-tbdb");
	        dic.Add("gender", 1);
	        dic.Add("code", "serwe");
            dic.Add("channel", "serwe");
            dic.Add("unique_id", "serwe");

			HttpUtil.Http.Post(URLManager.registerMobileUrl).Form(dic).OnSuccess(result =>
			{
				if (result != null)
				{
					AuthModel authModel = JsonMapper.ToObject<AuthModel>(result);
					if (authModel.ret == 1)
					{
						UserManager.Instance().authModel = authModel;
                        LoginHandle.authSuccess(action);
					}
					else
					{
						if (action != null)
							action(new Error(authModel.ret, authModel.msg), null);
					}
				}
			}).OnFail(result =>
			{
				if (action != null)
					action(new Error(500, null), null);
            }).GoSync();
		}
    }
}
