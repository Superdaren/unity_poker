using UnityEngine;
using System.Collections;
using System;
using LitJson;

public class SignInHandle
{
	/**
     * 获取签到列表
     */
    public static void requestSignInList(Action<Error, SignInResult> action)
	{
        HttpUtil.Http.Get(URLManager.signInListUrl()).OnSuccess(result =>
		{
			if (result != null)
			{
				SignInResult signInResult = JsonMapper.ToObject<SignInResult>(result);
				action(null, signInResult);
			}
		}).OnFail(result =>
		{
			action(new Error(500, null), null);
        }).Go();
	}

	/**
     * 领取签到奖励
     */
    public static void receiveSignInReward(int isMore, Action<Error, ReceiveSignInResult> action)
	{
        HttpUtil.Http.Get(URLManager.receiveSignInRewardUrl(isMore)).OnSuccess(result =>
		{
			if (result != null)
			{
                ReceiveSignInResult receiveSignInResult = JsonMapper.ToObject<ReceiveSignInResult>(result);
				action(null, receiveSignInResult);
			}
		}).OnFail(result =>
		{
			action(new Error(500, null), null);
        }).Go();
	}
}
