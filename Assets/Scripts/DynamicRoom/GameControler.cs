using DG.Tweening;
using NetProto;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NetProto;
using Google.Protobuf.Collections;

public class GameControler : MonoBehaviour
{
    public int maxCount;       // 参与人数
    private bool pcardSend;    // 判断是否已经发了三张公共牌
    private bool isShowLoading;// 是否显示加载视图

    public List<GameObject> PlayerObjs { get { return playerObjs; } }

    private List<GameObject> playerObjs = new List<GameObject>();   // 所有玩家集合
    private List<GameObject> handcards = new List<GameObject>();    // 手牌集合
    private List<GameObject> chips = new List<GameObject>();        // 所有筹码位置集合
    private List<GameObject> dealers = new List<GameObject>();      // 所有庄家Icon位置集合
    private List<GameObject> publicCards = new List<GameObject>();  // 公共牌集合
    private List<Vector3> publicCardsPos = new List<Vector3>();     // 公共牌位置集合

    private GameObject desktop;         // 桌面
    private GameObject beauty;          // 荷官
    private GameObject deck;            // 牌组
    private GameObject roomLight;       // 灯光
    private GameObject pool;            // 底池位置
    private GameObject dealer;          // 庄家Icon位置
    private GameObject waitting;        // 显示等待发牌组件
    private GameObject mLoadingObj;     // 换桌加载组件
    private GameObject publicCardObj;   // 公共排位置组件

    private Text tvPoolCount;           // 底池筹码Text

    private ButtonControler mButtonControler;   // 自己的控制台（可操作按钮控制器）
    public PlayerInfo selfInfo;                 // 自己的信息
    public TableInfo tableInfo;                 // 牌桌的信息
    public GameHandle mGameHandle { get; set; } // 请求的Handle

    // Use this for initialization
    void Start()
    {
        // 初始化组件
        InitComponent();
        // 初始化玩家位置
        //InitPlayer();
        // 初始化公共牌的位置
        InitPublicCard();
    }

    /*-------------------------------------------*/

    // 初始化组件
    private void InitComponent()
    {
        // 自己的控制台（可操作按钮控制器）
        mButtonControler = GetComponent<ButtonControler>();

        desktop = GameObject.Find("Desktop");
        beauty = GameObject.Find("beauty");
        deck = GameObject.Find("deck");
        roomLight = GameObject.Find("roomLight");
        pool = GameObject.Find("pool");
        waitting = GameObject.Find("waitting");
        mLoadingObj = GameObject.Find("ChangeTableLoading");
        publicCardObj = GameObject.Find("publicCard");
        tvPoolCount = GameObject.Find("poolCount").GetComponent<Text>();

        TransformUtil.SetWarpAnchor(beauty);
        deck.SetActive(false);
        roomLight.SetActive(false);
        waitting.SetActive(false);
        mLoadingObj.SetActive(false);
    }

    // 初始化玩家位置
    private void InitPlayer()
    {
        if (playerObjs.Count > 0)
        {
            return;
        }
        int slide_count = maxCount / 2;
        Vector3[] playerPos = Coordinate.GetPlayerPosArray(maxCount);
        Vector3[] chipPos = Coordinate.GetChipPosArray(maxCount);
        for (int i = 0; i < maxCount; i++)
        {
            // 初始化筹码位置
            string chipPath = i > slide_count ? "Prefabs/ChipLeft" : "Prefabs/ChipRight";
            GameObject chipObj = Instantiate(Resources.Load(chipPath)) as GameObject;
            chipObj.name = "chip" + i;
            chipObj.transform.parent = desktop.transform;
            chipObj.transform.localScale = new Vector3(1, 1, 1);
            chipObj.transform.localPosition = chipPos[i];
            chips.Add(chipObj);
            TransformUtil.SetWarpAnchor(chipObj);
            // 初始化玩家
            GameObject player = Instantiate(Resources.Load("Prefabs/Player")) as GameObject;
            player.name = "player" + i;
            player.transform.parent = desktop.transform;
            player.transform.localScale = new Vector3(1, 1, 1);
            player.GetComponent<PlayerControler>().chipObj = chipObj;
            player.GetComponent<PlayerControler>().SetHandCardPosition(i > slide_count);
            player.transform.localPosition = playerPos[i];
            playerObjs.Add(player);
            TransformUtil.SetWarpAnchor(player);
        }
        roomLight.GetComponent<LightControler>().SetPalyerObjects(playerObjs);
    }

    // 初始化公共牌的位置
    private void InitPublicCard()
    {
        for (int i = 0; i < 5; i++)
        {
            Vector3 vect = publicCardObj.transform.position;
            vect.x = vect.x + i * 1.5f;
            publicCardsPos.Add(vect);
        }
    }


    #region 房间的信息更新

    // 获取玩家在当前桌面上的座位[将玩家的的座位号转换成客户端本地坐标]
    // pos 表示当前player在PlayerInfo数组中的位置
    private int GetPlayerPos(int pos)
    {
        if (selfInfo.Pos > 0)
        {
            int inv = (maxCount / 2) - (selfInfo.Pos - 1);
            int target = pos + inv;
            if (target > maxCount - 1)
            {
                target = target - maxCount;
            }
            else if (target < 0)
            {
                target = target + maxCount;
            }
            return target;
        }
        else
        {
            return pos;
        }
    }

    // 初始化Info
    public void InitInfo(PlayerInfo selfInfo, TableInfo tableInfo)
    {
        this.selfInfo = selfInfo;
        this.tableInfo = tableInfo;
        // 初始化房间最大人数
        maxCount = tableInfo.Max;
        // 初始化玩家位置
        InitPlayer();
    }

    // 初始化玩家信息
    public void InitPlayerInfo(RepeatedField<PlayerInfo> playerInfos)
    {
        for (int i = 0; i < playerInfos.Count; i++)
        {
            int target = GetPlayerPos(i);
            playerObjs[target].GetComponent<PlayerControler>().PlayerInfo = playerInfos[i];
        }
    }

    // 初始化玩家手牌
    public void InitPlayerHandCard(RepeatedField<PlayerInfo> playerInfos)
    {
        deck.SetActive(true);
        int pos = 0;
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < playerInfos.Count; j++)
            {
                if (playerInfos[j].Pos == 0 || playerInfos[j].Cards == null || playerInfos[j].Cards.Count == 0)
                {
                    continue;
                }
                GameObject card = GameObject.Instantiate(deck, deck.transform.parent);
                card.name = "handcard" + pos;
                int target = GetPlayerPos(j);// 获取玩家对于的本地座位号
                card.GetComponent<CardControler>().player = playerObjs[target];
                playerObjs[target].GetComponent<PlayerControler>().AddHandcard(card);
                //判断是否是左边的手牌，i==0 的时候 is_left = false
                card.GetComponent<CardControler>().is_left = (i == 0);
                handcards.Add(card);
                pos++;
            }
        }
        foreach (var handcard in handcards)
        {
            GameObject playerObj = handcard.GetComponent<CardControler>().player;
            // 为了确保card能够Find 因此需要将SittedObject显示。
            playerObj.GetComponent<PlayerControler>().SetBaseContentActive(true);
            GameObject card = playerObj.GetComponent<PlayerControler>().GetCardObject;
            if (playerObj.GetComponent<PlayerControler>().PlayerInfo.Id == selfInfo.Id)
            {
                handcard.GetComponent<CardControler>().SetSelfCardPosition(card.transform.position);
            }
            else
            {
                handcard.GetComponent<CardControler>().SetCardPosition(card.transform.position);
            }
        }
        deck.SetActive(false);
    }

    // 初始化玩家已下筹码
    public void InitPlayerChips(RepeatedField<PlayerInfo> playerInfos)
    {
        for (int i = 0; i < playerInfos.Count; i++)
        {
            int target = GetPlayerPos(i);
            playerObjs[target].GetComponent<PlayerControler>().InitChips(playerInfos[i].Bet);
        }
    }

    // 初始化公共牌信息
    public void InitPublicCardInfo(RepeatedField<CardInfo> cardInfos)
    {
        if (cardInfos == null)
        {
            return;
        }
        for (int i = 0; i < cardInfos.Count; i++)
        {
            GameObject pcard = GameObject.Instantiate(deck, publicCardObj.transform);
            pcard.SetActive(true);
            pcard.name = "publicCard" + i;
            pcard.transform.position = publicCardsPos[i];
            pcard.transform.localScale = new Vector3(2, 2, 2);
            pcard.GetComponent<CardControler>().SetCardValue(cardInfos[i]);
            pcard.GetComponent<CardControler>().ShowCard();
            publicCards.Add(pcard);
        }
    }

    // 初始化底池
    public void InitPotInfo(RepeatedField<int> pot)
    {
        pool.GetComponent<PoolControler>().CreatePool(pot, true);
    }

    // 光标转到当前玩家
    public void TrunCurrentPalyer(RepeatedField<PlayerInfo> playerInfos, int bet)
    {
        for (int i = 0; i < playerInfos.Count; i++)
        {
            if (playerInfos[i].Action != null && playerInfos[i].Action.Equals(PlayerInfoUtil.ACTION_BETTING))
            {
                TurnNext(playerInfos[i].Pos, bet);
                break;
            }
        }
    }

    // 新玩家进入房间后更新UI
    public void UpdateAfterPlayerJoin(PlayerInfo newPlayerInfo)
    {
        tableInfo.N++;
        // 更新玩家数组
        tableInfo.Players[newPlayerInfo.Pos - 1] = newPlayerInfo;
        int target = GetPlayerPos(newPlayerInfo.Pos - 1);
        playerObjs[target].GetComponent<PlayerControler>().PlayerInfo = newPlayerInfo;
        if (tableInfo.N == 2)
        {
            SetPlayerReadyAction(tableInfo.Players);
        }
    }

    // 玩家离开房间后更新UI
    public void UpdateAfterPlayerGone(PlayerInfo playerInfo)
    {
        UpdateAfterPlayerGone(playerInfo.Pos);
    }

    // 玩家离开房间后更新UI
    public void UpdateAfterPlayerGone(int pos)
    {
        if (pos != selfInfo.Pos)
        {
            tableInfo.N--;
            if (tableInfo.N < 2)
            {
                waitting.SetActive(false);
            }
            // 更新玩家数组
            int target = GetPlayerPos(pos - 1);
            playerObjs[target].GetComponent<PlayerControler>().PlayerInfo = null;
            tableInfo.Players[pos - 1].Pos = 0;
        }
    }

    // 玩家坐下
    public void UpdateAfterPlayerSitDown(PlayerInfo newPlayerInfo)
    {
        if (newPlayerInfo.Id == selfInfo.Id)
        {
            // 更新坐下玩家数据
            UpdateSitdownPlayer(newPlayerInfo);
            // 重置UI
            ResetUIAfterStandOrSit();
            // 初始化玩家信息
            InitPlayerInfo(tableInfo.Players);
            if (tableInfo.N > 2)
            {
                // 初始化玩家手牌
                InitPlayerHandCard(tableInfo.Players);
                // 初始化玩家已下筹码
                InitPlayerChips(tableInfo.Players);
                // 光标转到当前玩家
                TrunCurrentPalyer(tableInfo.Players, tableInfo.Bet);
            }
        }
        else
        {
            UpdateAfterPlayerJoin(newPlayerInfo);
        }
    }

    // 玩家站起
    public void UpdateAfterPlayerStandUp(RoomPlayerStandupAck roomPlayerStandupAck)
    {
        if (roomPlayerStandupAck.PlayerId == selfInfo.Id)
        {
            if (roomPlayerStandupAck.Force)
            {
                PopUtil.ShowMessageBoxWithConfirm("", "由于您多次没操作，是不是太累了，先站起旁观吧！！！");
            }
            // 更新站起玩家的数据
            UpdateStandupPlayer(roomPlayerStandupAck);
            // 重置UI
            ResetUIAfterStandOrSit();
            // 初始化玩家信息
            InitPlayerInfo(tableInfo.Players);
            // 初始化玩家手牌
            InitPlayerHandCard(tableInfo.Players);
            // 初始化玩家已下筹码
            InitPlayerChips(tableInfo.Players);
            if (tableInfo.N >= 2)
            {
                // 光标转到当前玩家
                TrunCurrentPalyer(tableInfo.Players, tableInfo.Bet);
            }
            else
            {
                waitting.SetActive(false);
            }
        }
        else
        {
            UpdateAfterPlayerGone(roomPlayerStandupAck.PlayerPos);
        }
    }

    #endregion


    #region 游戏状态的控制

    // 设置庄家
    public void SetDealer(int player_pos)
    {
        Vector3[] dealerPos = Coordinate.GetDealerPosArray(maxCount);
        int target = GetPlayerPos(player_pos - 1);
        if (dealer == null)
        {
            string dealerPath = "Prefabs/Dealer";
            dealer = Instantiate(Resources.Load(dealerPath)) as GameObject;
            dealer.transform.parent = desktop.transform;
            dealer.transform.localScale = new Vector3(1, 1, 1);
            dealer.transform.localPosition = dealerPos[target];
            TransformUtil.SetWarpAnchor(dealer);
        }
        else
        {
            // 显示庄家
            dealer.SetActive(true);
            Sequence s = DOTween.Sequence();
            s.Append(dealer.transform.DOLocalMove(dealerPos[target], 1));
            s.AppendCallback(() =>
            {
                TransformUtil.ResetAnchor(dealer, dealerPos[target]);
                dealer.GetComponent<Image>().SetNativeSize();
                TransformUtil.SetWarpAnchor(dealer);
            });
        }
    }

    // 初始化 handcards
    private void InitHandCard(RepeatedField<CardInfo> cardInfos)
    {
        deck.SetActive(true);
        int pos = 0;
        RepeatedField<PlayerInfo> playerInfos = tableInfo.Players;
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < playerInfos.Count; j++)
            {
                if (playerInfos[j].Pos == 0)
                {
                    continue;
                }
                int target = GetPlayerPos(j);// 获取玩家对于的本地座位号
                playerInfos[j].Action = PlayerInfoUtil.ACTION_READY;
                GameObject playerObj = playerObjs[target];
                GameObject card = GameObject.Instantiate(deck, playerObj.GetComponent<PlayerControler>().GetCardObject.transform.parent);
                card.name = "handcard" + pos;
                card.GetComponent<CardControler>().player = playerObjs[target];
                playerObjs[target].GetComponent<PlayerControler>().AddHandcard(card);
                //判断是否是左边的手牌，i==0 的时候 is_left = false
                card.GetComponent<CardControler>().is_left = (i == 0);
                if (target == maxCount / 2)
                {
                    // 更新手牌信息
                    playerObjs[target].GetComponent<PlayerControler>().UpdateCardInfo(cardInfos);
                }
                handcards.Add(card);
                pos++;
            }
        }
    }

    // 发手牌
    public void DealHandCard(RepeatedField<CardInfo> cardInfos, string cardType)
    {
        // 设置是第一回合
        mButtonControler.SetIsRoundOne(true);
        waitting.SetActive(false);
        if (handcards.Count <= 0)
        {
            InitHandCard(cardInfos);
        }
        for (int i = 0; i < handcards.Count; i++)
        {
            GameObject hc = handcards[i];
            GameObject card = hc.GetComponent<CardControler>().player.GetComponent<PlayerControler>().GetCardObject;
            Sequence s = DOTween.Sequence();
            s.Append(hc.transform.DOMove(card.transform.position, 0.2f));
            s.PrependInterval(i * 0.1f);
            if (i == handcards.Count - 1)
            {
                s.AppendCallback(() =>
                {
                    deck.SetActive(false);
                    // 旋转手牌
                    foreach (GameObject player in playerObjs)
                    {
                        if (selfInfo.Pos > 0)
                        {
                            player.GetComponent<PlayerControler>().SeeCard("player" + (maxCount / 2));
                        }
                        else
                        {
                            player.GetComponent<PlayerControler>().SeeCard("player");
                        }
                    }
                });
            }
        }
    }

    // 发公共牌
    public void DealPublicCard(RepeatedField<CardInfo> cardInfos, string cardType)
    {
        if (publicCards.Count >= 3 && !pcardSend)
        {
            StartCoroutine(IEnumeratorProvider.DelayToInvokeDo(() => { DealPublicCard(cardInfos, cardType); }, 1.5f));
        }
        else
        {
            // 设置当前牌型
            ShowSelfCardType(cardType);
            // 将玩家自己该回合的下注筹码清零
            ResetSelfBet();
            // 设置不是第一回合
            mButtonControler.SetIsRoundOne(false);
            if (publicCards.Count < 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    GameObject card = GameObject.Instantiate(deck, publicCardObj.transform);
                    // 为公共牌赋值
                    card.GetComponent<CardControler>().SetCardValue(cardInfos[i]);
                    // 添加到集合
                    publicCards.Add(card);
                    card.SetActive(true);
                    card.transform.DOScale(2, 0.5f);
                    Sequence s = DOTween.Sequence();
                    s.Append(card.transform.DOMove(publicCardsPos[0], 0.5f));
                    Vector3 pos = publicCardsPos[i];
                    s.AppendCallback(() =>
                    {
                        pcardSend = true;
                        Sequence s1 = DOTween.Sequence();
                        s1.Append(card.GetComponent<CardControler>().Flip());
                        s1.Append(card.transform.DOMove(pos, 0.5f));
                    });
                }
            }
            else
            {
                GameObject card = GameObject.Instantiate(deck, publicCardObj.transform);
                // 为公共牌赋值
                card.GetComponent<CardControler>().SetCardValue(cardInfos[cardInfos.Count - 1]);
                // 添加到集合
                publicCards.Add(card);
                card.SetActive(true);
                Sequence s = DOTween.Sequence();
                s.Append(card.transform.DOMove(publicCardsPos[publicCards.Count - 1], 0.5f));
                s.AppendCallback(() =>
                {
                    card.GetComponent<CardControler>().Flip();
                });
                card.transform.DOScale(2, 0.5f);
            }
        }
    }

    // 显示玩家自己的牌型
    public void ShowSelfCardType(string cardType)
    {
        PlayerControler player = playerObjs[maxCount / 2].GetComponent<PlayerControler>();
        player.ShowCardType(cardType);
    }

    // 覆盖玩家自己的牌型
    public void HideSelfCardType()
    {
        PlayerControler player = playerObjs[maxCount / 2].GetComponent<PlayerControler>();
        player.HideCardType();
    }


    // 玩家弃牌(自己调用)
    public void Discard()
    {
        // 隐藏牌型
        HideSelfCardType();
        // 修改展示的操作按钮类型
        mButtonControler.SetCurrentType(ButtonControler.NONE_HANDLE);
        mGameHandle.RoomPlayerBetReq(-1);
    }

    // 玩家弃牌（其他玩家调用）
    public void Discard(int position)
    {
        if (position == selfInfo.Pos)
        {
            // 修改展示的操作按钮类型
            mButtonControler.SetCurrentType(ButtonControler.NONE_HANDLE);
        }
        int target = GetPlayerPos(position - 1);
        playerObjs[target].GetComponent<PlayerControler>().Discard(deck);
    }

    // 玩家下注(自己调用)
    public void PlayerBetting(int chipCount)
    {
        mGameHandle.RoomPlayerBetReq(chipCount);
    }

    // 接收玩家下注的消息处理
    public void PlayerBetting(RoomPlayerBetAck playerBetAck)
    {
        // 更新下注玩家数据
        UpdatePlayerInfo(playerBetAck);
        int target = GetPlayerPos(playerBetAck.Pos - 1);
        mButtonControler.SetBaseBet(playerBetAck.Bet - selfInfo.Bet > 0 ? playerBetAck.Bet - selfInfo.Bet : 0);

        playerObjs[target].GetComponent<PlayerControler>().Betting(playerBetAck);
    }

    // 轮到下一个玩家
    // position-轮到玩家的座位号
    // 最小下注金额
    public void TurnNext(int position, int bet)
    {
        if (position <= 0)
        {
            return;
        }
        // 更新table的信息
        tableInfo.Bet = bet;
        tableInfo.Players[position - 1].Action = PlayerInfoUtil.ACTION_BETTING;

        roomLight.SetActive(true);
        int target = GetPlayerPos(position - 1);
        roomLight.GetComponent<LightControler>().Rotate(target);
        if (selfInfo.Pos > 0 && selfInfo.Pos == position)
        {
            // 修改展示的操作按钮类型
            mButtonControler.SetCurrentType(ButtonControler.BET_HANDLE);
            mButtonControler.SetBaseBet(bet - selfInfo.Bet > 0 ? bet - selfInfo.Bet : 0);
        }
        else
        {
            // 更新底部Button UI
            if (PlayerInfoUtil.IsPlaying(selfInfo))
            {
                // 修改展示的操作按钮类型
                mButtonControler.SetCurrentType(ButtonControler.AUTO_HANDLE);
            }
            else
            {
                // 修改展示的操作按钮类型
                mButtonControler.SetCurrentType(ButtonControler.NONE_HANDLE);
            }
        }
        // 设置计时
        for (int i = 0; i < playerObjs.Count; i++)
        {
            PlayerControler p = playerObjs[i].GetComponent<PlayerControler>();
            if (i == target)
            {
                p.trunYou = true;
            }
            else
            {
                p.trunYou = false;
            }
        }
    }

    // 筹码收入底池
    public void PutChip(RepeatedField<int> pot)
    {
        // 更新tableInfo的底池信息
        //tableInfo.Pot = pot;
        tableInfo.Pot.Clear();
        tableInfo.Pot.Add(pot);
        // 更新底池总数
        int totalCount = 0;
        foreach (var count in pot)
        {
            totalCount += count;
        }
        tvPoolCount.text = "底池: " + StringUtil.GetStringChip(totalCount);
        bool isCreate = false;
        // 筹码收入底池
        foreach (var player in playerObjs)
        {
            List<GameObject> chipFabs = player.GetComponent<PlayerControler>().GetChipFabs;
            player.GetComponent<PlayerControler>().SetChipActive(false);
            for (int i = 0; i < chipFabs.Count; i++)
            {
                var chipFab = chipFabs[i];
                Sequence s = DOTween.Sequence();
                s.AppendInterval(0.2f);
                s.Append(chipFab.transform.DOMove(pool.transform.position, 0.2f));
                s.AppendCallback(() =>
                {
                    player.GetComponent<PlayerControler>().ClearChips();
                });
                if (!isCreate)
                {
                    isCreate = true;
                    s.AppendCallback(() =>
                    {
                        pool.GetComponent<PoolControler>().CreatePool(pot, false);
                    });
                }
            }
        }
    }

    // 亮牌
    public void DisplayCard()
    {
        foreach (GameObject card in handcards)
        {
            if (card != null && card.GetComponent<CardControler>().player.GetComponent<PlayerControler>().is_active)
            {
                // 亮牌
                card.GetComponent<CardControler>().IsDisplay = true;
            }
        }
    }

    // 游戏结束，宣布游戏结果
    public void GameOver(RoomShowdownAck roomShowdownAction)
    {
        // 隐藏牌型
        HideSelfCardType();
        roomLight.SetActive(false);
        this.roomShowdownAction = roomShowdownAction;
        this.tableInfo = roomShowdownAction.Table;
        UpdatePlayerArrayInfo(tableInfo.Players);
        mButtonControler.SetCurrentType(ButtonControler.NONE_HANDLE);
        // 将玩家自己该回合的下注筹码清零
        ResetSelfBet();
        // 亮牌
        DisplayCard();
        // 发放奖池
        Invoke("GrantChips", 1f);
        // 通知所有玩家游戏结束
        for (int i = 0; i < playerObjs.Count; i++)
        {
            playerObjs[i].GetComponent<PlayerControler>().gameOver = true; // 游戏结束
            playerObjs[i].GetComponent<PlayerControler>().RoundOver();     // 结束所有玩家的回合
        }
    }

    RoomShowdownAck roomShowdownAction;

    // 发放奖池
    public void GrantChips()
    {
        // 隐藏庄家
        dealer.SetActive(false);
        // 发放奖池
        pool.GetComponent<PoolControler>().GrantChips(roomShowdownAction.Table.Chips, roomShowdownAction.PotList, playerObjs, GetPlayerPos);
        Invoke("ResetUI", 3);
    }

    // 重置UI
    public void ResetUI()
    {
        // 清空手牌
        foreach (var handcard in handcards)
        {
            Destroy(handcard);
        }
        handcards.Clear();
        foreach (var player in playerObjs)
        {
            player.GetComponent<PlayerControler>().ResetUI();
        }
        // 清空公共牌
        foreach (var publiccard in publicCards)
        {
            Destroy(publiccard);
        }
        publicCards.Clear();
        // 清空底池筹码
        pool.GetComponent<PoolControler>().ResetUI();
        // 修改状态
        mButtonControler.SetCurrentType(ButtonControler.NONE_HANDLE);
        // 重置底池数量显示
        tvPoolCount.text = "";
        // 更新玩家action为ready
        SetPlayerReadyAction(tableInfo.Players);
        // 隐藏灯光
        roomLight.SetActive(false);
        // 隐藏庄家
        dealer.SetActive(false);
        // 初始化pcardSend
        pcardSend = false;
    }

    // 站起或坐下时的UI重置
    public void ResetUIAfterStandOrSit()
    {
        // 清空手牌
        foreach (var handcard in handcards)
        {
            Destroy(handcard);
        }
        handcards.Clear();
        foreach (var player in playerObjs)
        {
            player.GetComponent<PlayerControler>().ResetUI();
            player.GetComponent<PlayerControler>().ClearChips();
        }
        // 修改状态
        mButtonControler.SetCurrentType(ButtonControler.NONE_HANDLE);
    }

    // 换桌时的UI重置
    public void ResetUIAfterChangeTable()
    {
        // 清空手牌
        foreach (var handcard in handcards)
        {
            Destroy(handcard);
        }
        handcards.Clear();
        foreach (var player in playerObjs)
        {
            player.GetComponent<PlayerControler>().ResetUI();
        }
        // 清空公共牌
        foreach (var publiccard in publicCards)
        {
            Destroy(publiccard);
        }
        publicCards.Clear();
        // 清空底池筹码
        pool.GetComponent<PoolControler>().ResetUI();
        // 修改状态
        mButtonControler.SetCurrentType(ButtonControler.NONE_HANDLE);
        // 重置底池数量显示
        tvPoolCount.text = "";
        // 隐藏灯光
        roomLight.SetActive(false);
        // 隐藏庄家
        dealer.SetActive(false);
    }

    #endregion


    #region 玩家数据更新

    // 每次牌局开始之前先清空底池
    private void ResetPot()
    {
        tableInfo.Pot.Clear();
    }

    // 将玩家自己该回合的下注筹码清零[发牌调用]
    private void ResetSelfBet()
    {
        if (selfInfo.Pos > 0)
        {
            selfInfo.Bet = 0;
            tableInfo.Players[selfInfo.Pos - 1].Bet = 0;
        }
    }

    // 将玩家状态置为准备状态
    public void SetPlayerReadyAction(RepeatedField<PlayerInfo> playerInfos)
    {
        for (int i = 0; i < playerInfos.Count; i++)
        {
            playerInfos[i].Action = PlayerInfoUtil.ACTION_READY;
        }
        // 在显示之前判断当前玩家是否大于1个人，大于显示，小于隐藏
        waitting.SetActive(tableInfo.N > 1);
    }

    // 更新下注玩家数据
    private void UpdatePlayerInfo(RoomPlayerBetAck playerBetAck)
    {
        tableInfo.Players[playerBetAck.Pos - 1].Action = playerBetAck.Action;
        tableInfo.Players[playerBetAck.Pos - 1].Chips = playerBetAck.Chips;
        tableInfo.Players[playerBetAck.Pos - 1].Bet = playerBetAck.Bet;
        UpdatePlayerArrayInfo(tableInfo.Players);
    }

    // 更新本地玩家信息
    public void UpdatePlayerArrayInfo(RepeatedField<PlayerInfo> playerInfos)
    {
        if (selfInfo.Pos > 0)
        {
            this.selfInfo = playerInfos[selfInfo.Pos - 1];
        }
        for (int i = 0; i < playerInfos.Count; i++)
        {
            int target = GetPlayerPos(i);
            playerObjs[target].GetComponent<PlayerControler>().PlayerInfo = playerInfos[i];
        }
    }

    // 更新站起玩家的数据
    private void UpdateStandupPlayer(RoomPlayerStandupAck roomPlayerStandupAck)
    {
        tableInfo.N--;
        if (roomPlayerStandupAck.PlayerId == selfInfo.Id)
        {
            selfInfo.Pos = 0;
        }
        tableInfo.Players[roomPlayerStandupAck.PlayerPos - 1].Pos = 0;
        tableInfo.Players[roomPlayerStandupAck.PlayerPos - 1].Bet = 0;
        int target = GetPlayerPos(roomPlayerStandupAck.PlayerPos - 1);
        playerObjs[target].GetComponent<PlayerControler>().PlayerInfo = null;
    }

    // 更新坐下玩家数据
    private void UpdateSitdownPlayer(PlayerInfo newPlayerInfo)
    {
        tableInfo.N++;
        if (newPlayerInfo.Id == selfInfo.Id)
        {
            selfInfo = newPlayerInfo;
        }
        tableInfo.Players[newPlayerInfo.Pos - 1] = newPlayerInfo;
        int target = GetPlayerPos(newPlayerInfo.Pos - 1);
        playerObjs[target].GetComponent<PlayerControler>().PlayerInfo = newPlayerInfo;
        if (tableInfo.N == 2)
        {
            SetPlayerReadyAction(tableInfo.Players);
        }
    }


    #endregion


    #region 加载页面类型

    // 显示/隐藏换桌加载
    public void SetChangeTableLoadingActive(bool active)
    {
        mLoadingObj.SetActive(active);
    }

    #endregion


    #region 聊天UI更新

    public void PlayerSay(int pos, ChatMessage chatM)
    {
        int target = GetPlayerPos(pos - 1);
        PlayerControler player = PlayerObjs[target].GetComponent<PlayerControler>();
        int bubble = 0;
        if (target > maxCount / 2)
        {
            bubble = PlayerControler.LEFT_BUBBLE;
        }
        else if (target < maxCount / 2)
        {
            bubble = PlayerControler.RIGHT_BUBBLE;
        }
        else
        {
            bubble = PlayerControler.CENTER_BUBBLE;
        }
        player.Say(bubble, chatM);
    }

    #endregion
}
