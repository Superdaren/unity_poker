using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class RoundControler : MonoBehaviour
{

    public GameObject cards;
    public GameObject card;
    public Text action;

    private List<GameObject> cardList = new List<GameObject>();

    private string format = "Textures/cards/card_{0}{1}";
    private string formatDefault = "Textures/cards/card_back";

    /**
     * 初始化视图
     */
    public void InitView(Card[] cards, PlayerAction playerAction, bool isShow)
    {
        // 初始化卡牌
        InitCard(cards, isShow);
        // 设置动作 
        SetAction(playerAction);
    }

    /**
     * 初始化卡牌
     */ 
    public void InitCard(Card[] cards, bool isShow)
    {
        cardList.Add(card);
        SetCardValue(card, cards[0], isShow);
        for (int i = 1; i < cards.Length; i++)
        {
            GameObject cardC = null;
            if (i < cardList.Count)
            {
                cardC = cardList[i];
            }
            else
            {
                cardC = GameObject.Instantiate(card, card.transform.parent);
                cardC.name = "card" + i;
                cardList.Add(cardC);
            }
            SetCardValue(cardC, cards[i], isShow);
        }
    }

    /**
     * 设置牌值 
     */
    private void SetCardValue(GameObject cardObj, Card card, bool isShow)
    {
        string path = isShow ? format : formatDefault;
        cardObj.GetComponent<Image>().SetLocalImage(string.Format(path, card.suit, card.value));
    }

    /**
     * 设置动作 
     */
    private void SetAction(PlayerAction playerAction)
    {
        if (playerAction == null)
        {
            action.text = "";
        }
        else
        {
            if (playerAction.bet > 0)
            {
                StringBuilder builder = new StringBuilder();
                action.text = builder.Append(playerAction.GetAction()).Append(playerAction.bet).ToString();
            }
            else
            {
                action.text = playerAction.GetAction();
            }
        }
    }
}
