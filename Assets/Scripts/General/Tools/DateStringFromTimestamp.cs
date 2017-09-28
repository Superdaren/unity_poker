using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class DateStringFromTimestamp
{
	// 计算当前时间戳与目标时间戳的时间间隔
	public static string DateStringFromNow(string dt)
	{
		string timeStamp = dt;
		DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
		long lTime = long.Parse(timeStamp + "0000000");
		TimeSpan toNow = new TimeSpan(lTime);
		DateTime dtResult = dtStart.Add(toNow);
		TimeSpan span = DateTime.Now - dtResult;
		if (span.TotalDays > 90)
		{
			return "3个月前";
		}
		else
		if (span.TotalDays > 60)
		{
			return "2个月前";
		}
		else if (span.TotalDays > 30)
		{
			return "1个月前";
		}
		else if (span.TotalDays > 14)
		{
			return "2周前";
		}
		else if (span.TotalDays > 7)
		{
			return "1周前";
		}

		else if (span.TotalDays > 1)
		{
			return string.Format("{0}天前", (int)Math.Floor(span.TotalDays));
								 
		}
		else if (span.TotalHours > 1)
		{
			return string.Format("{0}小时前", (int)Math.Floor(span.TotalHours));
		}
		else if (span.TotalMinutes > 1)
		{
			return string.Format("{0}分钟前", (int)Math.Floor(span.TotalMinutes));
		}
		else if (span.TotalSeconds >= 1)
		{
			return string.Format("{0}秒前",(int)Math.Floor(span.TotalSeconds));					 
		}

		else
		{
			return "1秒前";
		}
	}
}
