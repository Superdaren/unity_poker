
public class CardInfoUtil
{
    public const string ACTION_PREFLOP = "preflop";
    public const string ACTION_FLOP = "flop";
    public const string ACTION_TURN = "turn";
    public const string ACTION_RIVER = "river";

    public static string GetCardType(int hand_level)
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
                break;
        }
        return cardType;
    }
}
