using DG.Tweening;
using NetProto;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonControler : MonoBehaviour
{
    public const int NONE_HANDLE = 0;
    public const int AUTO_HANDLE = 1;
    public const int BET_HANDLE = 2;
    public const int RAISE_HANDLE = 3;

    public int currentType;                  // 当前的状态
    private bool auto_over;                  // 是否自动让或弃
    private bool auto_pass;                  // 是否自动让
    private bool auto_follow;                // 是否自动跟
    private int pot;                         // 底池总数
    private int bigBlind;                    // 大盲注
    private bool isRoundOne;                 // 是否是第一轮游戏
    private int totalChip;                   // 玩家自己所有的筹码
    private int baseBet = 0;                 // 当前下注额
    public GameHandle mGameHandle { get; set; } // 请求的Handle

    private GameControler mGameControler;
    private GameObject autoHandle, betHandle, raiseHandle;

    /**
     * 设置当前下注额
     */
    public void SetBaseBet(int baseBet)
    {
        this.baseBet = baseBet;
    }

    /**
     * 设置是否是第一回合
     */
    public void SetIsRoundOne(bool isRoundOne)
    {
        this.isRoundOne = isRoundOne;
    }

    // Use this for initialization
    void Start()
    {
        mGameControler = GetComponent<GameControler>();

        // 当现实加注时，设置点击空白处使raiseHandle部分隐藏，betHandle部分显示
        GameObject.Find("Desktop").GetComponent<Button>().onClick.AddListener(() =>
        {
            if (raiseHandle.active)
            {
                currentType = BET_HANDLE;
            }
            if (menuList.active)
            {
                PopMenu(false);
            }
            if (cardBoardObj.active)
            {
                PopCardBoard(false);
            }
            if (mFuncOptionsObj.active)
            {
                PopupChatView(false);
                PopupPreGameView(false);
            }
        });
    }

    private bool isFirest = true;

    // Update is called once per frame
    void Update()
    {
        // 每隔5帧执行一次
        if (Time.frameCount % 5 == 0 || isFirest)
        {
            isFirest = false;
            // 初始化Button
            InitComponent();
            // 更新数据
            UpdateData();
        }
    }

    /**
     * 初始化Button
     */
    private void InitComponent()
    {
        // 初始化自动模块
        InitAuto(currentType);
        // 初始化下注模块
        InitBet(currentType);
        // 初始化加注模块
        InitRaise(currentType);
        // 初始化自动坐下
        InitAutoSit();
        // 初始化功能选项
        InitFunctionOptions(currentType);
        // 初始菜单按钮
        InitMenu();
    }

    /**
     * 更新数据
     */
    public void UpdateData()
    {
        if (mGameControler.tableInfo == null || mGameControler.tableInfo.Pot == null || mGameControler.selfInfo.Pos <= 0)
        {
            return;
        }
        int temp = 0;
        foreach (var potItem in mGameControler.tableInfo.Pot)
        {
            temp += potItem;
        }
        pot = temp;
        bigBlind = mGameControler.tableInfo.BigBlind;
        totalChip = mGameControler.PlayerObjs[mGameControler.maxCount / 2].GetComponent<PlayerControler>().PlayerInfo.Chips;
        baseBet = baseBet > totalChip ? totalChip : baseBet;
    }

    /**
     * 修改当前状态
     */
    public void SetCurrentType(int type)
    {
        currentType = type;
        if (currentType == BET_HANDLE)
        {
            if (auto_over)
            {
                int count = baseBet == 0 ? 0 : -1;
                mGameControler.PlayerBetting(count);
            }
            else if (auto_pass || auto_follow)
            {
                mGameControler.PlayerBetting(baseBet);
                baseBet = 0;
            }
        }
    }

    #region 自动模块

    private Toggle autoOverToggle, autoPassToggle, autoFollowToggle;    // 自动操作的组件
    private GameObject autoOverBg, autoPassBg, autoFollowBg;            // 自动操作的组件
    private Text tvAutoPass;                                            // 自动让对应的文本

    private void InitAuto(int type)
    {
        if (autoHandle != null)
        {
            autoHandle.SetActive(type == AUTO_HANDLE);
            UpdateAutoToggle();
        }
        else
        {
            autoHandle = GameObject.Find("AutoHandle");
            tvAutoPass = GameObject.Find("autoPass/Toggle/Label").GetComponent<Text>();
            InitToggle();
            autoHandle.SetActive(type == AUTO_HANDLE);
        }
    }

    /**
     * 初始化Toggle触发事件
     */
    private void InitToggle()
    {
        // 自动让或弃
        autoOverBg = GameObject.Find("autoOver/select_bg");
        autoOverBg.SetActive(false);
        autoOverToggle = GameObject.Find("autoOver/Toggle").GetComponent<Toggle>();
        autoOverToggle.onValueChanged.AddListener((bool select) =>
        {
            auto_over = select;
            autoOverBg.SetActive(select);
        });
        // 自动让
        autoPassBg = GameObject.Find("autoPass/select_bg");
        autoPassBg.SetActive(false);
        autoPassToggle = GameObject.Find("autoPass/Toggle").GetComponent<Toggle>();
        autoPassToggle.onValueChanged.AddListener((bool select) =>
        {
            auto_pass = select;
            autoPassBg.SetActive(select);
        });
        // 自动跟
        autoFollowBg = GameObject.Find("autoFollow/select_bg");
        autoFollowBg.SetActive(false);
        autoFollowToggle = GameObject.Find("autoFollow/Toggle").GetComponent<Toggle>();
        autoFollowToggle.onValueChanged.AddListener((bool select) =>
        {
            auto_follow = select;
            autoFollowBg.SetActive(select);
        });
    }

    /**
     * 更新autoPassUI
     */
    public void UpdateAutoToggle()
    {
        string str = "";
        if (baseBet == 0)
        {
            str = "自动让牌";
        }
        else
        {
            str = "跟" + baseBet;
        }
        tvAutoPass.text = str;
        autoOverToggle.isOn = false;
        autoPassToggle.isOn = false;
        autoFollowToggle.isOn = false;
    }

    #endregion

    #region 下注模块

    // 左边三个按钮的集合
    private List<GameObject> btMulpools = new List<GameObject>();
    // MulpoolButton的文本数组
    string[] values1 = { "3x大盲", "4x大盲", "1x底池" };
    string[] values2 = { "1/2底池", "2/3底池", "1x底池" };
    string btFollowFormat1 = "让牌";
    string btFollowFormat2 = "跟注 {0}";

    /**
     * 初始化下注模块
     */
    private void InitBet(int type)
    {
        if (betHandle != null)
        {
            betHandle.SetActive(type == BET_HANDLE);
            if (type == BET_HANDLE)
            {
                UpdateMulpools();
            }
        }
        else
        {
            betHandle = GameObject.Find("BetHandle");
            InitMulpoolButton();
            //弃牌点击监听
            GameObject.Find("BetHandle/CommonHandle/btDiscard").GetComponent<Button>().onClick.AddListener(() =>
            {
                mGameControler.Discard();
                currentType = NONE_HANDLE;
                // 播放声音
                AudioUtil.Play(AudioUtil.Click);
            });
            // 跟注点击监听
            string str = baseBet == 0 ? btFollowFormat1 : string.Format(btFollowFormat2, baseBet);
            GameObject.Find("BetHandle/CommonHandle/btFollow/Text").GetComponent<Text>().text = str;
            GameObject.Find("BetHandle/CommonHandle/btFollow").GetComponent<Button>().onClick.AddListener(() =>
            {
                mGameControler.PlayerBetting(baseBet);
                currentType = AUTO_HANDLE;
                // 播放声音
                AudioUtil.Play(AudioUtil.Click);
            });
            // 加注点击监听
            GameObject.Find("BetHandle/CommonHandle/btRaise").GetComponent<Button>().onClick.AddListener(() =>
            {
                currentType = RAISE_HANDLE;
                // 播放声音
                AudioUtil.Play(AudioUtil.Click);
            });
            betHandle.SetActive(type == BET_HANDLE);
        }
    }

    /**
     * 初始化 MulpoolButton
     */
    private void InitMulpoolButton()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject btMulpool = GameObject.Find("btMulpool" + i);
            float temp = i;
            btMulpool.GetComponent<Button>().onClick.AddListener(() =>
            {
                int count = 0;
                if (isRoundOne)
                {
                    count = (int)((temp + 3) > 4 ? pot : (temp + 3) * bigBlind);
                }
                else
                {
                    count = (int)((temp + 1) > 2 ? pot : ((temp + 1) / (temp + 2)) * pot);
                }
                mGameControler.PlayerBetting(count > totalChip ? totalChip : count);
                currentType = AUTO_HANDLE;
            });
            btMulpools.Add(btMulpool);
            UpdateMulpools();
        }
    }

    /**
     * 更新UI
     */
    private void UpdateMulpools()
    {
        // 更新让牌||跟注文本
        string str = baseBet == 0 ? btFollowFormat1 : string.Format(btFollowFormat2, baseBet);
        GameObject.Find("BetHandle/CommonHandle/btFollow/Text").GetComponent<Text>().text = str;
        // 更新底池倍数按钮UI
        for (int i = 0; i < btMulpools.Count; i++)
        {
            // 更新按钮文本
            string s = isRoundOne ? values1[i] : values2[i];
            GameObject.Find(btMulpools[i].name + "/Text").GetComponent<Text>().text = s;
            BtnEnabledControler mControler = btMulpools[i].GetComponent<BtnEnabledControler>();
            // 更新按钮的enabled,控制按钮的点击
            if (isRoundOne)
            {
                // 第一回合，x倍盲注按钮可点击，x倍底池不可点击
                mControler.SetEnabled(i != btMulpools.Count - 1);
            }
            else
            {
                // 当前下注额大于底池，都不可以点击
                if (baseBet > pot)
                {
                    mControler.SetEnabled(false);
                }
                // 当前下注额大于2/3底池，只有1倍底池可以点击
                else if (baseBet > (2 / 3 * pot))
                {
                    mControler.SetEnabled(i == btMulpools.Count - 1);
                }
                // 当前下注额大于1/2底池，2/3倍和1倍底池可以点击
                else if (baseBet > (1 / 2 * pot))
                {
                    mControler.SetEnabled(i != 0);
                }
                // 当前下注额小于1/2底池，都可以点击
                else if (pot < (1 / 2 * pot))
                {
                    mControler.SetEnabled(true);
                }
            }
        }
    }

    #endregion

    #region 加注模块

    private Image sliderChipImage;           // 筹码加注器背景图
    private Slider slider;                   // 筹码加注器
    private Text tvCount;                    // 筹码加注器对应的文本

    private int[] values = { 200, 500, 1000, 2000, 5000 };
    private List<GameObject> btFixedChips = new List<GameObject>();
    private bool checkable = true; // 是否需要检测按钮是否可点击

    /**
     * 初始化加注模块
     */
    private void InitRaise(int type)
    {
        if (raiseHandle != null)
        {
            raiseHandle.SetActive(type == RAISE_HANDLE);
            if (raiseHandle.active)
            {
                if (checkable)
                {
                    // 设置筹码加注器最低下注额
                    tvCount.text = baseBet.ToString();

                    slider.value = baseBet > 0 ? (baseBet * 1f / totalChip) : 0;
                    // 设置固定金额的加注按钮是否可点击
                    for (int i = 0; i < btFixedChips.Count; i++)
                    {
                        bool enabled = values[i] >= baseBet && values[i] <= totalChip;
                        btFixedChips[i].GetComponent<BtnEnabledControler>().SetEnabled(enabled);
                    }
                }
            }
            else
            {
                checkable = true;
            }
        }
        else
        {
            raiseHandle = GameObject.Find("RaiseHandle");
            // 初始化固定筹码按钮
            InitFixedChip();
            // 初始化可调节筹码加注器
            InitAdjustChip();
            raiseHandle.SetActive(type == RAISE_HANDLE);
        }
    }

    /**
     * 初始化固定筹码按钮
     */
    private void InitFixedChip()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject btFixedChip = GameObject.Find("btFixedChip" + i);
            GameObject.Find(btFixedChip.name + "/Text").GetComponent<Text>().text = values[i].ToString();
            int temp = values[i];
            btFixedChip.GetComponent<Button>().onClick.AddListener(() =>
            {
                mGameControler.PlayerBetting(temp);
                currentType = AUTO_HANDLE;
                // 播放声音
                AudioUtil.Play(AudioUtil.Click);
            });
            btFixedChips.Add(btFixedChip);
        }
    }

    /**
     * 初始化可调节筹码加注器
     */
    private void InitAdjustChip()
    {
        int count = 0;
        // 初始化按钮的监听
        Button btCount = GameObject.Find("RaiseHandle/AdjustableChip/btCount").GetComponent<Button>();
        btCount.onClick.AddListener(() =>
        {
            mGameControler.PlayerBetting(count);
            currentType = AUTO_HANDLE;
            // 播放声音
            AudioUtil.Play(AudioUtil.Click);
        });
        // 初始化Slider的监听
        sliderChipImage = GameObject.Find("SliderChip").GetComponent<Image>();
        slider = GameObject.Find("Slider").GetComponent<Slider>();
        tvCount = GameObject.Find("RaiseHandle/AdjustableChip/btCount/Text").GetComponent<Text>();
        // 设置筹码加注器最低下注额
        tvCount.text = baseBet.ToString();
        slider.value = (baseBet * 1f / totalChip);
        slider.onValueChanged.AddListener((float value) =>
        {
            if (checkable)
            {
                checkable = false;
                count = baseBet;
                tvCount.text = baseBet.ToString();
            }
            else
            {
                count = (int)(totalChip * value);
                tvCount.text = count.ToString();
            }
            string sliderBg = value == 1 ? "Textures/room/betting_seekbar_allin_bg" : "Textures/room/betting_seekbar_bg";
            sliderChipImage.SetLocalImage(sliderBg);
            // 如何当前筹码数小于最低下注额，则按钮不能点击，即不能下注
            GameObject.Find("RaiseHandle/AdjustableChip/btCount").GetComponent<BtnEnabledControler>().SetEnabled(count >= baseBet);
        });
    }
    #endregion

    #region 坐下模块

    private GameObject mSitdown;             // 坐下组件
    private GameObject mAutoSitDownObj;      // 自动坐下组件
    private GameObject mWaittingSitObj;      // 等待坐下组件
    private GameObject waitCountObj;         // 显示等待坐下人数组件
    private Image sitdownBgImage;            // 坐下按钮的背景Image
    private Text waitCountText;              // 显示等待坐下人数Text(点击自动坐下之前)
    private Text waittingSitText;            // 显示等待坐下人数Text(点击自动坐下之后)
    public bool isFull = false;
    private string format = "等待坐下，前面还有{0}人";

    /**
     * 初始化自动坐下按钮
     */
    private void InitAutoSit()
    {
        if (mSitdown != null)
        {
            bool active = mGameControler.selfInfo != null && mGameControler.selfInfo.Pos <= 0;
            // 如果自动坐下显示，则站起菜单按钮应该不可点击
            btStandUp.interactable = (!active && mGameControler.tableInfo.N > 2);
            if (active)
            {
                mSitdown.SetActive(true);
                mAutoSitDownObj.SetActive(true);
                mWaittingSitObj.SetActive(false);
                // 修改按钮样式
                isFull = IsFull();
                string path = isFull ? "Textures/other/game_auto_sit_font" : "Textures/other/game_sitdown_font";
                waitCountObj.SetActive(isFull);
                sitdownBgImage.SetLocalImage(path);
                sitdownBgImage.SetNativeSize();
            }
            else
            {
                mSitdown.SetActive(false);
            }
        }
        else
        {
            mSitdown = GameObject.Find("Sitdown");
            mAutoSitDownObj = GameObject.Find("AutoSitDown");
            mWaittingSitObj = GameObject.Find("WaittingSit");
            waitCountObj = GameObject.Find("waitCount");
            sitdownBgImage = GameObject.Find("sitdownBg").GetComponent<Image>();
            waitCountText = waitCountObj.GetComponent<Text>();
            waittingSitText = GameObject.Find("waittingSitText").GetComponent<Text>();
            mWaittingSitObj.SetActive(false);
            // 坐下点击实现
            mAutoSitDownObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                // 播放声音
                AudioUtil.Play(AudioUtil.Click);
                if (mGameHandle != null && mGameControler.selfInfo.Pos <= 0)
                {
                    if (isFull)
                    {
                        mGameHandle.RoomPlayerAutoSitdownReq();
                        mAutoSitDownObj.SetActive(false);
                        mWaittingSitObj.SetActive(true);
                    }
                    else
                    {
                        mGameHandle.RoomPlayerSitdownReq();
                    }
                }
            });
        }
    }

    /**
     * 更新等待人数
     */
    public void UpdateWaitCount(int count)
    {
        waitCountText.text = count.ToString();
        waittingSitText.text = string.Format(format, count);
    }

    /**
     * 判断房间座位是否坐满
     */
    private bool IsFull()
    {
        bool isFull = false;
        if (mGameControler.tableInfo.N >= mGameControler.tableInfo.Max)
        {
            isFull = true;
        }
        return isFull;
    }

    #endregion

    #region 功能选项

    private GameObject mFuncOptionsObj;      // 功能选项
    private GameObject mChatViewObj;         // 聊天视图
    private GameObject mLastGameViewObj;     // 上局回顾视图
    private GameObject lastGameObj;          // 上局回顾内容视图
    private Vector3 lastGamePos;

    /**
     * 初始化功能选项
     */
    public void InitFunctionOptions(int type)
    {
        if (mFuncOptionsObj == null)
        {
            mFuncOptionsObj = GameObject.Find("FunctionOptions");
            mChatViewObj = GameObject.Find("ChatView");
            mLastGameViewObj = GameObject.Find("LastGameView");
            lastGameObj = GameObject.Find("LastGame");
            lastGamePos = mLastGameViewObj.transform.localPosition;
            mChatViewObj.SetActive(false);
            GameObject.Find("chatOption").GetComponent<Button>().onClick.AddListener(() =>
            {
                // 播放声音
                AudioUtil.Play(AudioUtil.Click);
                PopupChatView(true);
            });
            GameObject.Find("preGameOption").GetComponent<Button>().onClick.AddListener(() =>
            {
                // 播放声音
                AudioUtil.Play(AudioUtil.Click);
                PopupPreGameView(true);
                mGameHandle.GetLastGame(mGameControler.tableInfo.Id, (error, result) =>
                {
                    if (error == null && result != null)
                    {
                        lastGameObj.GetComponent<LastGameAdapter>().SetData(result.data);
                    }
                });
            });
        }
        else
        {
            mFuncOptionsObj.SetActive(type != BET_HANDLE);
            if (!mFuncOptionsObj.active)
            {
                PopupChatView(false);
                PopupPreGameView(false);
            }
        }
    }

    /**
     * 弹出、收回聊天窗口
     */
    public void PopupChatView(bool active)
    {
        if (active)
        {
            mChatViewObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            mChatViewObj.SetActive(true);
            mChatViewObj.transform.DOScale(new Vector3(1, 1, 1), 0.5f);
        }
        else
        {
            Sequence s = DOTween.Sequence();
            s.Append(mChatViewObj.transform.DOScale(new Vector3(0f, 0f, 0f), 0.5f));
            s.AppendCallback(() =>
            {
                mChatViewObj.SetActive(false);
            });
        }
    }

    /**
     * 弹出、收回上局回顾窗口
     */
    public void PopupPreGameView(bool active)
    {
        if (active)
        {
            mLastGameViewObj.SetActive(active);
            mLastGameViewObj.transform.DOLocalMoveY(0, 0.5f);
        }
        else
        {
            Sequence s = DOTween.Sequence();
            s.Append(mLastGameViewObj.transform.DOLocalMoveY(lastGamePos.y, 0.5f));
            s.AppendCallback(() =>
            {
                mLastGameViewObj.SetActive(active);
            });
        }
    }

    #endregion

    #region 下拉菜单

    private GameObject menuList, menu, cardBoardObj;
    private Button btChangeTable, btStandUp;
    private Vector3 menuPos, boardPos;

    private void InitMenu()
    {
        if (menuList != null)
        {
            if (mGameControler.tableInfo == null)
            {
                return;
            }
            // 更新换桌按钮是否可点击（正在游戏中不能点击）
            bool enabled = mGameControler.tableInfo.N < 2 || !PlayerInfoUtil.IsPlaying(mGameControler.selfInfo);
            btChangeTable.interactable = enabled;
        }
        else
        {
            menu = GameObject.Find("Menu");
            menuList = GameObject.Find("MenuList");
            cardBoardObj = GameObject.Find("cardBoard");
            menuPos = menuList.transform.localPosition;
            boardPos = cardBoardObj.transform.localPosition;
            // 菜单按钮的点击实现
            GameObject.Find("switch").GetComponent<Button>().onClick.AddListener(() =>
            {
                // 播放声音
                AudioUtil.Play(AudioUtil.Click);
                PopMenu(true);
            });
            // 返回按钮的点击实现
            GameObject.Find("back").GetComponent<Button>().onClick.AddListener(() =>
            {
                if (mGameHandle != null)
                {
                    // 播放声音
                    AudioUtil.Play(AudioUtil.Click);
                    mGameHandle.RoomPlayerGoneReq();
                    Application.LoadLevel("main");
                }
            });
            // 换桌点击实现
            btChangeTable = GameObject.Find("changeTable").GetComponent<Button>();
            btChangeTable.onClick.AddListener(() =>
            {
                if (!PlayerInfoUtil.IsPlaying(mGameControler.selfInfo))
                {
                    // 播放声音
                    AudioUtil.Play(AudioUtil.Click);
                    PopMenu(false);
                    // 隐藏换桌加载组件
                    mGameControler.SetChangeTableLoadingActive(true);
                    mGameHandle.RoomPlayerChangeTableReq();
                }
            });
            // 站起点击实现
            btStandUp = GameObject.Find("standUp").GetComponent<Button>();
            btStandUp.onClick.AddListener(() =>
            {
                if (mGameHandle != null && mGameControler.selfInfo.Pos > 0)
                {
                    // 播放声音
                    AudioUtil.Play(AudioUtil.Click);
                    mGameHandle.RoomPlayerStandupReq();
                    PopMenu(false);
                }
            });
            GameObject.Find("cardType").GetComponent<Button>().onClick.AddListener(() =>
            {
                // 播放声音
                AudioUtil.Play(AudioUtil.Click);
                PopMenu(false);
                PopCardBoard(true);
            });
            menuList.SetActive(false);
        }
    }

    /**
     * 弹出、收回菜单
     */
    private void PopMenu(bool active)
    {
        if (active)
        {
            menuList.SetActive(active);
            menuList.transform.DOLocalMoveY(0, 0.5f);
        }
        else
        {
            Sequence s = DOTween.Sequence();
            s.Append(menuList.transform.DOLocalMoveY(menuPos.y, 0.5f));
            s.AppendCallback(() =>
            {
                menuList.SetActive(active);
            });
        }
    }

    /**
     * 弹出、收回牌型板
     */
    private void PopCardBoard(bool active)
    {
        if (active)
        {
            cardBoardObj.SetActive(active);
            cardBoardObj.transform.DOLocalMoveX(0, 0.5f);
        }
        else
        {
            Sequence s = DOTween.Sequence();
            s.Append(cardBoardObj.transform.DOLocalMoveX(boardPos.x, 0.5f));
            s.AppendCallback(() =>
            {
                cardBoardObj.SetActive(active);
            });
        }
    }

    #endregion
}
