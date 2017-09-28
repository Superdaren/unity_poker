using UnityEngine;
using System.Collections;

/**
 * 用户信息表
 */
public class UserInfo
{
    // id
    public int id;
    // 头像
    public string avatar;
	// 金币余额
	public int balance;
	// 钻石余额 
	public int diamond_balance;
	// 性♂别 0 未知 1男2女
	public int gender;
	// 是否新用户
	public int is_fresh;
	// 手机号
	public string mobile_number;
	// 昵称 
	public string nick_name;
	// 状态
	public int status;
	// 用户类型
	public int type;
	// 上次签到时间 
	public string last_checkin_time;
	// 签到天数
	public int checkin_days;
    // 当天是否签到
    public int is_checkin;
}
