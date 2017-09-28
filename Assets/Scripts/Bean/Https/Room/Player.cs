using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Player
{
    /** 
     *  获取玩家牌型
     *  这套手牌的权重等级，一共有10个等级
     *  皇家同花顺：10
     *  同花顺    ：9
     *  四条      ：8
     *  葫芦      ：7
     *  同花      ：6
     *  顺子      ：5
     *  三条      ：4
     *  两对      ：3
     *  一对      ：2
     *  高牌      ：1
     */
    public string GetCardType()
    {
        string cardType = "";
        switch (hand_level)
        {
            case 1:
                cardType = "高牌";
                break;
            case 2:
                cardType = "对子";
                break;
            case 3:
                cardType = "两对";
                break;
            case 4:
                cardType = "三条";
                break;
            case 5:
                cardType = "顺子";
                break;
            case 6:
                cardType = "同花";
                break;
            case 7:
                cardType = "葫芦";
                break;
            case 8:
                cardType = "四条";
                break;
            case 9:
                cardType = "同花顺";
                break;
            case 10:
                cardType = "皇家同花顺";
                break;
            default:
                cardType = "";
                break;
        }
        return cardType;
    }


    //玩家id 
    public Int32 id;         
    //位置            
    public Int32 pos;               
    // 玩家显示名
    public string nickname;
    // 玩家头像
    public string avatar;
    // 玩家等级
    public string level;
    // 当前筹码
    public Int32 current_chips;
    // 开始筹码 
    public Int32 former_chips;
    // 投注金额
    public Int32 bet;
    // 赢取金额
    public Int32 win;
    // 玩家当前行动(ready, betting, check, call, raise, fold, flee)，初始状态为ready, 中途加入的玩家状态为空
    public string action;
    // 牌值
    public Int32 hand_final_value;
    // 牌型
    public Int32 hand_level;
    // 玩家底牌，其他玩家只有摊牌时才能看到
    public Card[] cards;
    // 玩家操作记录
    public PlayerAction[] actions;    
}
