using NetProto;

/**
 * 玩家信息
 */
public class PlayerInfoUtil
{
    public const string ACTION_STANDUP = "standup";     // 站起
    public const string ACTION_SITDOWN = "sitdown";     // 坐下
    public const string ACTION_READY = "ready";         // 准备
    public const string ACTION_BETTING = "betting";     // 下注中
    public const string ACTION_CHECK = "check";         // 让牌
    public const string ACTION_CALL = "call";           // 跟注
    public const string ACTION_RAISE = "raise";         // 加注
    public const string ACTION_FLOD = "fold";           // 弃牌
    public const string ACTION_ALLIN = "allin";         // 全押

    // 判断当前玩家是否正在游戏中
    public static bool IsPlaying(PlayerInfo info)
    {
        bool isPlaying = false;
        if (info.Pos > 0 && !string.IsNullOrEmpty(info.Action))
        {
            switch (info.Action)
            {
                case ACTION_STANDUP:
                case ACTION_SITDOWN:
                case ACTION_FLOD:
                    isPlaying = false;
                    break;
                default:
                    isPlaying = true;
                    break;
            }
        }
        return isPlaying;
    }

    // 获取玩家牌型
    public static string GetCardType(PlayerInfo info)
    {
        string cardType = "";
        switch (info.HandLevel)
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
                cardType = "逃跑";
                break;
        }
        return cardType;
    }

}
