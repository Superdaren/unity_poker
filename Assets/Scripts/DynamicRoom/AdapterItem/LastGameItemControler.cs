using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LastGameItemControler : BaseAdapterItem<Player>
{
    public GameObject Rounds;
    public GameObject totalCount;
    public GameObject role;
    public GameObject cardType;

    public Text roleText;
    public Text cardTypeText;
    public Image avatar;

    private LastGame lastGame;

    // round组件集合
    private List<GameObject> roundList = new List<GameObject>();

    // 初始化数据
    public override void SetData(Player data, params object[] objs)
    {
        if (objs != null)
        {
            lastGame = (LastGame)objs[0];
        }
        // 显示角色
        string roleStr = "";
        if (data.pos == lastGame.sb_pos)
        {
            roleStr = "小盲";
        }
        else if (data.pos == lastGame.bb_pos)
        {
            roleStr = "大盲";
        }
        if (data.pos == lastGame.button)
        {
            roleStr = string.IsNullOrEmpty(roleStr) ? "D" : "D / " + roleStr;
        }
        if (string.IsNullOrEmpty(roleStr))
        {
            role.SetActive(false);
        }
        else
        {
            role.SetActive(true);
            roleText.text = roleStr;
        }
        // 显示牌型
        cardType.SetActive(!string.IsNullOrEmpty(data.GetCardType()));
        cardTypeText.text = data.GetCardType();

        // 下注总数
        if (data.win > 0)
        {
            totalCount.SetActive(true);
            totalCount.GetComponent<Text>().text = "+" + data.win.ToString();
        }
        else if (data.bet > 0)
        {
            totalCount.SetActive(true);
            totalCount.GetComponent<Text>().text = "-" + data.bet.ToString();
        }
        else
        {
            totalCount.SetActive(false);
        }
        InitRounds(data, data.actions);
    }

    public void InitRounds(Player player, PlayerAction[] actions)
    {
        bool isShowAll = true;  // 是否显示所有回合的牌
        for (int i = 0; i < actions.Length; i++)
        {
            // 如果公共牌不足5张且不出现fold、flee，则显示全部
            if (lastGame.cards.Length < 5)
            {
                isShowAll = false;
            }
            if (actions[i] != null && (actions[i].action.Equals("fold") || actions[i].action.Equals("flee")))
            {
                isShowAll = false;
            }

            // 玩家获胜，但是在该回合玩家没有做其他操作的情况，则继续执行一次for循环
            int roundIndex = lastGame.cards.Length - 2;
            bool isContinue = roundIndex > 0 && player.win > 0 && i == roundIndex;

            if (i != 0 && actions[i] == null && !isContinue && !isShowAll)
            {
                roundIndex = roundIndex > 0 ? roundIndex + 1 : i;
                for (int j = roundIndex; j < roundList.Count; j++)
                {
                    roundList[j].SetActive(false);
                }
                break;
            }
            Card[] cards = null;
            switch (i)
            {
                case 0:
                    cards = player.cards;
                    break;
                case 1:
                    cards = new Card[] { lastGame.cards[0], lastGame.cards[1], lastGame.cards[2] };
                    break;
                case 2:
                    cards = new Card[] { lastGame.cards[3] };
                    break;
                case 3:
                    cards = new Card[] { lastGame.cards[4] };
                    break;
            }
            GameObject round = null;
            if (i < roundList.Count)
            {
                round = roundList[i];
            }
            else
            {
                round = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Round"));
                round.name = "round" + i;
                round.transform.parent = Rounds.transform;
                round.transform.localScale = new Vector3(1, 1, 1);
                roundList.Add(round);
            }
            // 如果是玩家自己或者该玩家坚持到牌局结束，则显示(只针对第一回合的判断，其他回合都显示)
            bool isShow = i != 0 
                || isShowAll
                || actions[actions.Length - 1] != null 
                || player.id == UserManager.Instance().authModel.user_id;
            round.SetActive(true);
            round.GetComponent<RoundControler>().InitView(cards, actions[i], isShow);
        }
    }
}
