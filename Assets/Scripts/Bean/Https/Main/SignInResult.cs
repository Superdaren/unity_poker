using UnityEngine;
using System.Collections;

/**
 * 获取签到列表结果
 */
public class SignInResult
{
	public int ret;

	public string msg;

    public SignIn[] list;

	/** 已经签到天数 **/
    public int already_checkin;
	/** 上次签到时间  **/
	public string last_checkin_time;
	/** 是否已经领取7天之后的额外奖励 1为已领取  **/
    public int is_more;
}
