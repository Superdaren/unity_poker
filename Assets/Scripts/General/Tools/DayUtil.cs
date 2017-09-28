using UnityEngine;
using System.Collections;

public class DayUtil
{
    /**
     * 根据数数据获取天数
     */
    public static string getChineseDayWithInt(int day) {
        switch (day) {
            case 1: 
            {
                return "第一天";
            }
			case 2:
			{
                return "第二天";
			}
			case 3:
			{
                return "第三天";
			}
			case 4:
			{
                return "第四天";
			}
			case 5:
			{
                return "第五天";
			}
			case 6:
			{
                return "第六天";
			}
			case 7:
			{
				return "第七天";
			}
            default:
                return "七天后";
        }
    }
}
