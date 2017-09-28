using DG.Tweening;
using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NetProto;

public class PlayerControler : MonoBehaviour
{

    // 玩家位置的状态
    private const int COMMON_STATUS = 1;
    private const int NONE_STATUS = 2;

    // 说话发出气泡的类型
    public const int LEFT_BUBBLE = 1;
    public const int RIGHT_BUBBLE = 2;
    public const int CENTER_BUBBLE = 3;

    public int status = COMMON_STATUS;  // 当前玩家位置的状态
    public bool trunYou;                // 是否轮到该玩家
    public bool gameOver;               // 是否游戏结束
    public bool is_active = false;      // 玩家是否处于活跃状态
    public bool is_initclip = false;    // 是否已经初始化已下筹码
    public GameObject chipObj;          // 对应筹码的组件

    private GameObject mBaseContentObj; // 基本内容父组件
    private GameObject shadowObj;       // 阴影组件
    private GameObject cardObj;         // 手牌的位置组件
    private GameObject chipFabObj;      // 筹码位置的组件
    private GameObject progressObj;     // 倒计时背景的组件
    private GameObject actionObj;       // 玩家状态组件
    private GameObject nicNameObj;      // 玩家昵称组件

    private GameObject winViewObj;      // 获胜背景组件
    private GameObject cardTypeObj;     // 牌型组件
    private GameObject bubblePosObj;    // 气泡位置组件
    private GameObject selfCardTypeObj; // 自己当前的牌型

    private Image progressImage;        // 倒计时背景的组件下的image
    private Image cardTypeImage;        // 牌型image
    private Image actionImage;          // 牌型image
    private Image avtarImage;           // 用户头像组件image

    private Text nickNameText;          // 玩家昵称Text 
    private Text chipCountText;         // 筹码数量   
    private float CDTime = 20;          // 当前倒计时时间

    private PlayerInfo mPlayerInfo;     // 玩家信息

    //十、百、1千、5千、10万、50万
    private string[] chipArray = {
        "Textures/room/chips_yellow_img",
        "Textures/room/chips_green_img",
        "Textures/room/chips_orange_img",
        "Textures/room/chips_purple_img",
        "Textures/room/chips_red_img",
        "Textures/room/chips_blue_img",
        "Textures/room/chips_black_img"
    };
    private List<Sprite> chipList = new List<Sprite>();         // 下注的筹码图标
    private List<GameObject> chipFabs = new List<GameObject>(); // 下注的筹码组件
    private List<GameObject> handcards = new List<GameObject>();// 手牌

    // 设置玩家信息
    public PlayerInfo PlayerInfo
    {
        get { return mPlayerInfo; }
        set { mPlayerInfo = value; }
    }

    // 获取手牌的位置组件
    public GameObject GetCardObject
    {
        get
        {
            if (cardObj == null)
            {
                cardObj = GameObject.Find(name + "/Content/card");
            }
            return cardObj;
        }
    }

    // 获取筹码位置的组件
    public GameObject GetChipFab { get { return chipFabObj; } }

    // 获取所有筹码
    public List<GameObject> GetChipFabs { get { return chipFabs; } }

    // 添加手牌
    public void AddHandcard(GameObject handcard)
    {
        handcards.Add(handcard);
    }

    // 设置手牌的位置
    public void SetHandCardPosition(bool is_left)
    {
        if (is_left)
        {
            if (cardObj == null)
            {
                cardObj = GameObject.Find(name + "/Content/card");
            }
            Vector3 localPos = cardObj.transform.localPosition;
            localPos.x = -localPos.x * 2f / 3f;
            cardObj.transform.localPosition = localPos;
        }
    }

    // Use this for initialization
    void Start()
    {
        // 初始化组件
        InitComponentt();
        // 设置玩家信息
        UpdatePlayerInfo(mPlayerInfo);
    }

    // Update is called once per frame
    void Update()
    {
        // 每隔5帧执行一次
        if (Time.frameCount % 5 == 0)
        {
            // 设置玩家信息
            UpdatePlayerInfo(mPlayerInfo);
            // 更新阴影显示
            shadowObj.SetActive(!is_active);
        }
    }

    // Update is called once fixed time
    private void FixedUpdate()
    {
        if (trunYou)
        {
            RoundStart();
        }
        else
        {
            RoundOver();
        }
    }

    // 初始化组件
    private void InitComponentt()
    {
        mBaseContentObj = GameObject.Find(name + "/Content");
        string path = name + "/" + mBaseContentObj.name;
        shadowObj = GameObject.Find(path + "/shadow");
        cardObj = GameObject.Find(path + "/card");
        chipFabObj = GameObject.Find(path + "/chipFab");
        progressObj = GameObject.Find(path + "/progress");
        actionObj = GameObject.Find(path + "/action");
        nicNameObj = GameObject.Find(path + "/nickName");

        winViewObj = GameObject.Find(name + "/WinView");
        cardTypeObj = GameObject.Find(name + "/CardType");
        bubblePosObj = GameObject.Find(name + "/bubblePos");
        selfCardTypeObj = GameObject.Find(name + "/selfCardType");

        progressImage = progressObj.GetComponent<Image>();
        cardTypeImage = GameObject.Find(cardTypeObj.name + "/cardType").GetComponent<Image>();
        actionImage = actionObj.GetComponent<Image>();
        avtarImage = GameObject.Find(path + "/avtar").GetComponent<Image>();

        nickNameText = nicNameObj.GetComponent<Text>();
        chipCountText = GameObject.Find(path + "/chipCount").GetComponent<Text>();

        // 如果已经初始化已下筹码则显示，反之隐藏
        if (chipObj != null)
        {
            chipObj.SetActive(is_initclip);
        }
        chipFabObj.SetActive(false);
        progressObj.SetActive(false);
        winViewObj.SetActive(false);
        cardTypeObj.SetActive(false);
        actionObj.SetActive(false);
        selfCardTypeObj.SetActive(false);
    }

    // 设置chip是否显示
    public void SetChipActive(bool isActive)
    {
        chipObj.SetActive(isActive);
    }

    // 设置 mSittedObject 显示
    public void SetBaseContentActive(bool active)
    {
        if (mBaseContentObj != null)
        {
            mBaseContentObj.SetActive(active);
        }
    }

    // 更新手牌信息
    public void UpdateCardInfo(RepeatedField<CardInfo> cardInfos)
    {
        if (cardInfos != null && cardInfos.Count > 0)
        {
            mPlayerInfo.Cards.Clear();
            mPlayerInfo.Cards.Add(cardInfos);
        }
    }

    // 更新玩家信息
    public void UpdatePlayerInfo(PlayerInfo info)
    {
        if (info == null || info.Pos == 0)
        {
            status = NONE_STATUS;
            // 清空手牌
            ClearHandCards();
        }
        else
        {
            status = COMMON_STATUS;
        }
        // 显示Content的UI
        mBaseContentObj.SetActive(status == COMMON_STATUS);
        if (status == COMMON_STATUS)
        {
            // 初始化手牌
            if (info.Cards != null && info.Cards.Count > 0)
            {
                for (int i = 0; i < handcards.Count; i++)
                {
                    handcards[i].GetComponent<CardControler>().SetCardValue(info.Cards[i]);
                }
            }
            chipCountText.text = info.Chips.ToString();
            if (gameOver && info.HandLevel > 0)
            {
                cardTypeObj.SetActive(true);
                cardTypeImage.SetLocalImage("Textures/room/card_type_" + info.HandLevel);
                cardTypeImage.SetNativeSize();
                // 显示用户名
                nicNameObj.SetActive(true);
                actionObj.SetActive(false);
                nickNameText.text = info.Id.ToString();
            }
            else
            {   // 显示状态
                switch (info.Action)
                {
                    case PlayerInfoUtil.ACTION_BETTING:
                    case PlayerInfoUtil.ACTION_CHECK:
                    case PlayerInfoUtil.ACTION_CALL:
                    case PlayerInfoUtil.ACTION_RAISE:
                    case PlayerInfoUtil.ACTION_ALLIN:
                        nicNameObj.SetActive(false);
                        actionObj.SetActive(true);
                        actionImage.SetLocalImage("Textures/room/action_" + info.Action);
                        actionImage.SetNativeSize();
                        is_active = true;
                        break;
                    case PlayerInfoUtil.ACTION_FLOD:
                        nicNameObj.SetActive(false);
                        actionObj.SetActive(true);
                        actionImage.SetLocalImage("Textures/room/action_" + info.Action);
                        actionImage.SetNativeSize();
                        is_active = false;
                        break;
                    case PlayerInfoUtil.ACTION_READY:
                        nicNameObj.SetActive(true);
                        actionObj.SetActive(false);
                        nickNameText.text = info.Id.ToString();
                        is_active = true;
                        break;
                    default:
                        nicNameObj.SetActive(true);
                        actionObj.SetActive(false);
                        nickNameText.text = info.Id.ToString();
                        is_active = false;
                        break;
                }
            }
        }
    }

    // 回合结束(翻牌)
    public void RoundOver()
    {
        trunYou = false;
        progressImage.fillAmount = 1;
        progressObj.SetActive(false);
    }

    // 回合开始
    private void RoundStart()
    {
        progressObj.SetActive(true);
        if (progressImage.fillAmount > 0)
        {
            progressImage.fillAmount -= (1f / CDTime) * Time.deltaTime;
        }
        else
        {
            RoundOver();// 回合结束
        }
    }

    // 下注
    public void Betting(RoomPlayerBetAck playerBetAck)
    {
        CreateChips(playerBetAck.Bet);
    }

    // 下注
    public void Betting(int chipCount)
    {
        CreateChips(chipCount);
    }

    // 初始化已下（筹码刚进入房间时候调用）
    public void InitChips(int count)
    {
        if (count <= 0)
        {
            chipObj.SetActive(false);
            return;
        }
        if (chipFabObj == null)
        {
            chipFabObj = GameObject.Find(name + "/Content/chipFab");
        }
        chipFabObj.SetActive(true);
        chipObj.SetActive(true);
        InitChipList(count);
        for (int i = 0; i < chipList.Count; i++)
        {
            GameObject item = GameObject.Instantiate(chipFabObj, transform);
            item.name = "chipFabObj" + (i + 1);
            item.GetComponent<Image>().sprite = chipList[i];
            Vector3 v = chipObj.GetComponent<ChipControler>().GetChipIcon().transform.position;
            v.y = v.y + (i * 0.02f);
            item.transform.position = v;
            chipFabs.Add(item);
        }
        chipObj.GetComponent<ChipControler>().ChangeChip(count);
        chipFabObj.SetActive(false);
        is_initclip = true;
    }

    // 创建筹码
    private void CreateChips(int count)
    {
        if (count <= 0)
        {
            return;
        }
        InitChipList(count);
        chipObj.SetActive(true);
        chipFabObj.SetActive(true);
        RoundOver();// 回合结束
        int unit = count / chipList.Count;
        for (int i = 0; i < chipList.Count; i++)
        {
            int price1 = i == chipList.Count - 1 ? count : unit * (i + 1);
            GameObject item = GameObject.Instantiate(chipFabObj, transform);
            item.name = "chipFabObj" + (i + 1);
            item.GetComponent<Image>().sprite = chipList[i];
            Sequence s = DOTween.Sequence();
            s.PrependInterval(i * 0.1f);
            Vector3 v = chipObj.GetComponent<ChipControler>().GetChipIcon().transform.position;
            v.y = v.y + (i * 0.02f);
            s.Append(item.transform.DOMove(v, 0.1f));
            s.AppendCallback(() =>
            {
                chipObj.GetComponent<ChipControler>().ChangeChip(price1);
            });
            chipFabs.Add(item);
            if (i == chipList.Count - 1)
            {
                chipFabObj.SetActive(false);
            }
        }
    }

    // 初始化筹码Icon列表
    public void InitChipList(int count)
    {
        chipList.Clear();
        int time = count.ToString().Length - 1; // 获取10的n次方
        //int maxChip = time > chipArray.Length ? chipArray.Length : time;
        if (time < 1)
        {
            Sprite sprite = Resources.Load(chipArray[0], typeof(Sprite)) as Sprite;
            chipList.Add(sprite);
        }
        else
        {
            for (int i = time; i > 0; i--)
            {
                int digit = (int)(count / Math.Pow(10, i));
                count = (int)(count % Math.Pow(10, i));

                Sprite sprite = Resources.Load(chipArray[i - 1], typeof(Sprite)) as Sprite;

                for (int j = 0; j < digit; j++)
                {
                    chipList.Add(sprite);
                }
            }
        }
    }

    // 弃牌(其他玩家调用)
    public void Discard(GameObject deck)
    {
        is_active = false;
        chipObj.SetActive(false);
        // 修改玩家action
        mPlayerInfo.Action = PlayerInfoUtil.ACTION_FLOD;
        foreach (GameObject item in handcards)
        {
            Sequence s = DOTween.Sequence();
            s.Append(item.transform.DOMove(deck.transform.position, 0.5f));
            s.AppendCallback(() =>
            {
                item.SetActive(false);
            });
            item.transform.DOScale(0.1f, 0.5f);
        }
        RoundOver();
    }

    // 看牌
    public void SeeCard(string slef_name)
    {
        foreach (GameObject obj in handcards)
        {
            if (name.Equals(slef_name))
            {
                obj.GetComponent<CardControler>().SelfFlip();
            }
            else
            {
                obj.GetComponent<CardControler>().OtherFlip();
            }
        }
    }

    // 显示自己的牌型
    public void ShowCardType(string cardType)
    {
        selfCardTypeObj.SetActive(true);
        selfCardTypeObj.GetComponent<Text>().text = cardType;
    }

    // 隐藏自己的牌型
    public void HideCardType()
    {
        selfCardTypeObj.SetActive(false);
    }

    // 清除筹码
    public void ClearChips()
    {
        foreach (GameObject item in chipFabs)
        {
            Destroy(item);
        }
        chipFabs.Clear();
        chipList.Clear();
        chipObj.SetActive(false);
    }

    // 清除手牌(该函数只在玩家离开房间时候调用)
    private void ClearHandCards()
    {
        for (int i = 0; i < handcards.Count; i++)
        {
            Destroy(handcards[i]);
            handcards[i] = null;
        }
        handcards.Clear();
    }

    // 说话、聊天
    public void Say(int bubble, ChatMessage chatM)
    {
        if (chatM.type == ChatMessage.EMOJI)
        {
            bubblePosObj.transform.localPosition = Vector3.zero;
            GameObject emojiObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/EmojiItem"), transform.parent);
            emojiObj.transform.position = bubblePosObj.transform.position;
            int index = int.Parse(chatM.message.Split('_')[1]);
            emojiObj.GetComponent<Image>().sprite = Resources.LoadAll<Sprite>("Textures/room/gameChat")[index];
            emojiObj.GetComponent<Image>().SetNativeSize();
            emojiObj.transform.localScale = Vector3.zero;
            Sequence s = DOTween.Sequence();
            s.Append(emojiObj.transform.DOScale(new Vector3(1, 1, 1), 1f));
            s.AppendInterval(1.5f);
            s.AppendCallback(()=>
            {
                Destroy(emojiObj);
            });
        }
        else
        {
            float w = mBaseContentObj.GetComponent<RectTransform>().sizeDelta.x;
            float h = mBaseContentObj.GetComponent<RectTransform>().sizeDelta.y;
            GameObject bubbleObj = null;
            if (bubble == LEFT_BUBBLE)
            {
                bubblePosObj.transform.localPosition = new Vector3(w / 2, h / 4);
                bubbleObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/BubbleLeft"), transform.parent);
            }
            else if (bubble == RIGHT_BUBBLE)
            {
                bubblePosObj.transform.localPosition = new Vector3(-w / 2, h / 4);
                bubbleObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/BubbleRight"), transform.parent);
            }
            else
            {
                bubblePosObj.transform.localPosition = new Vector3(0, h / 2);
                bubbleObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/BubbleCenter"), transform.parent);
            }
            bubbleObj.transform.position = bubblePosObj.transform.position;
            bubbleObj.GetComponent<BubbleControler>().SetText(chatM.message);
        }
    }

    // 获胜
    public void Win(int count)
    {
        trunYou = false;
        progressImage.fillAmount = 1;
        progressObj.SetActive(false);
        winViewObj.SetActive(true);
        CreateWinCount(count);
    }

    // 创建显示赢取筹码数量的组件
    private void CreateWinCount(long count)
    {
        GameObject tvCount = Instantiate(Resources.Load("Prefabs/WinText")) as GameObject;
        tvCount.transform.parent = transform;
        tvCount.transform.localPosition = Vector3.zero;
        tvCount.transform.localScale = new Vector3(1, 1, 1);
        Text text = tvCount.GetComponent<Text>();
        text.text = "+" + count;

        Sequence s = DOTween.Sequence();
        s.Append(tvCount.transform.DOLocalMoveY(180, 1.5f));
        s.AppendInterval(1f);
        s.AppendCallback(() =>
        {
            Destroy(tvCount);
        });
    }

    // 重置UI
    public void ResetUI()
    {
        try
        {
            gameOver = false;
            // 清除筹码
            ClearChips();
            // 清空手牌
            handcards.Clear();
            // 重置倒计时进度
            progressObj.SetActive(false);
            // 重置获胜背景
            winViewObj.SetActive(false);
            // 重置牌型显示
            cardTypeObj.SetActive(false);
            //重置牌型
            mPlayerInfo.HandLevel = -1;
        }
        catch (Exception e)
        {
            print(e.Message);
        }
    }

}
