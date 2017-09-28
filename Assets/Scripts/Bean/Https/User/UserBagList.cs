using System.Collections.Generic;

/**
 * 用户背包列表
 */
public class UserBagList
{
	public int ret;
	public string msg;
    public int user_id;
    public List<BagItem> list;
}

public class BagItem{
	// 商品类别 
	public int goods_category_id;
	// 描述
	public string goods_describe;
	// 商品id
	public int goods_id;
	// 商品属性
	public int goods_type_id;
	// 图片
    public string image;
	// 0为永久,其他为过期时间 时间戳 
	public int is_expire;
	// 商品名 
	public string name;
	// 数量
	public int number;
}

