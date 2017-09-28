using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class PlayerAction
{
    /**
     * 获取玩家动作
     */
    public string GetAction()
    {
        string actionStr = "";
        switch (action)
        {
            case "standup":
                actionStr = "站起";
                break;
            case "sitdown":
                actionStr = "坐下";
                break;
            case "ready":
                actionStr = "准备";
                break;
            case "betting":
                actionStr = "下注中";
                break;
            case "check":
                actionStr = "让牌";
                break;
            case "call":
                actionStr = "跟注";
                break;
            case "raise":
                actionStr = "加注";
                break;
            case "fold":
                actionStr = "弃牌";
                break;
            case "allin":
                actionStr = "全押";
                break;
            case "flee":
                actionStr = "逃跑";
                break;
            default:
                break;
        }
        return actionStr;
    }

    /** 玩家动作 **/
    public string action;
    /** 回合下注金额 **/
    public Int32 bet;
}
