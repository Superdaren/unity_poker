using UnityEngine;
using System.Collections.Generic;
using System;

public class MainData
{
	static MainData mainData = null;
	// 牌桌信息
    public NetProto.TableInfo tableInfo;

    public NetProto.PlayerInfo selfInfo;

    //  钻石列表
    public Diamond[] diamondList;
    // 商品列表
    public ChargeGoodsInfo[] chargeGoods;

    public int roomId = 1;

	public static MainData Instance()
	{
		if (mainData == null)
		{
			mainData = new MainData();
		}
		return mainData;
	}

    /**
     * 根据钻石类型的id获取价格
     */
    public int getGoodsPrice(string productID) {
        foreach(Diamond diamond in diamondList) {
            if(diamond.charge_goods_id == productID){
                return diamond.price;
            }
        }

        return 0;
    }
}
