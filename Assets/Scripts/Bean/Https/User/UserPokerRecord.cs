using System.Collections.Generic;

/**
 * 用户牌局记录
 */
public class UserPokerRecord
{
	public int ret; // 1成功 0失败 
	public string msg;
	public int count;
	public List<RecordItem> list;
}

public class RecordItem
{
    public int order_id; // 局数
    public int bb; // 大盲注
    public int bb_pos; // 大盲注位置
	public int button; // 庄家
	public List<Cards> cards; // 牌
	public int end; // 结束时间
	public int max; // 最多玩家数
	public List<Players> players; // 玩家
	public List<int> pot; // 奖池
	public int room_id; // 房间id
	public int sb; // 小盲注
	public int sb_pos; // 小盲注位置
	public int start; // 开始时间
	public string table_id; // 桌子id

}
public class Cards
{
    public int suit; // 花色
	public int value; // 牌值
}
public class Players
{
    public string action; // 动作
	public List<Actions> actions; // 四轮动作
	public string avatar; // 头像
	public int bet; // 投注金额
	public List<Cards> cards; // 手牌
	public int current_chips; // 当前筹码
	public int former_chips; // 开始筹码
	public int hand_final_value; // 牌值
	public int hand_level; // 牌等级
	public int id; // 玩家id
	public string nickname; // 昵称
	public int pos; // 位置
	public int win; // 赢取金额
}
public class Actions
{
    public string action; // 动作
	public int bet; // 该轮总下注
}

