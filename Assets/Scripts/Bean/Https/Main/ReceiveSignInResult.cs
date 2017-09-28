using UnityEngine;
using System.Collections;

/**
 * 领取签到奖励
 */
public class ReceiveSignInResult
{
	public int ret;

	public string msg;

	/** 签到天数 **/
    public int checkin_days;

	/** 上次签到时间 **/
    public string last_checkin_time;
}
