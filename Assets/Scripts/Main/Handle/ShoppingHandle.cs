using UnityEngine;
using System.Collections.Generic;
using System;
using LitJson;

public class ShoppingHandle
{
    /**
     * 获取钻石列表
     */
    public static void getDiamondList(Action<Error, DiamondList> action)
	{
        HttpUtil.Http.Get(URLManager.diamondListUrl).OnSuccess(result =>
		{
			if (result != null)
			{
                DiamondList goodsList = JsonMapper.ToObject<DiamondList>(result);
				action(null, goodsList);
			}
		}).OnFail(result =>
		{
			action(new Error(500, null), null);
        }).Go();
	}

	/**
     * 获取商品列表
     */
    public static void getChargeGoodsList(Action<Error, ChargeGoodsResult> action)
	{
        HttpUtil.Http.Get(URLManager.chargeGoodsListUrl).OnSuccess(result =>
		{
			if (result != null)
			{
                ChargeGoodsResult goodsList = JsonMapper.ToObject<ChargeGoodsResult>(result);
				action(null, goodsList);
			}
		}).OnFail(result =>
		{
			action(new Error(500, null), null);
        }).Go();
	}

	/**
     * 钻石兑换
     */
    public static void exchargeDiamond(string goodID, Action<Error> action)
	{
        HttpUtil.Http.Get(URLManager.diamondExchangeUrl(goodID)).OnSuccess(result =>
		{
			Dictionary<string, object> resultDict = JsonMapper.ToObject<Dictionary<string, object>>(result);
			string ret = resultDict["ret"].ToString();
            if(ret == "1") {
                action(null);
            } else {
                action(new Error(500, null));
            }
		}).OnFail(result =>
		{
			action(new Error(500, null));
        }).Go();
	}
}
