using System.Collections.Generic;

/**
 * 用户详细信息
 */
public class UserDetail
{
	public int ret;
	public string msg;
	// 最大赢取筹码
	public int best_winner;
	// 最大牌
	public List<UserBestCards> cards;
	// 经验
	public int experience;
	// 等级
	public int grade;
	// 等级描述
	public string grade_describe;
	// 下一级所要求经验 
	public int next_experience;
	// 入局率
	public double inbound_rate;
	// 摊牌率
	public double showdown_rate;
	// 总局数
	public int total_game;
	// 胜率
	public double win_rate;
}

// 最大牌
public class UserBestCards
{
	// 花色
	public int suit;
	// 大小
	public int value;
}
