using NetProto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatControler : MonoBehaviour
{
    public GameHandle mGameHandle { get; set; } // 请求的Handle
    private GameControler mGameControler;
    private ButtonControler mButtonControler;

    // 类型按钮组件
    private GameObject chatTypeObj;
    private GameObject emojiTypeObj;
    private GameObject chatLogTypeObj;

    // 内容组件
    private GameObject chatObj;
    private GameObject emojiObj;
    private GameObject chatLogObj;

    private ChatAdapter mChatAdapter;
    private EmojiAdapter mEmojiAdapter;
    private ChatLogAdapter mChatLogAdapter;

    List<ChatMessage> messages = new List<ChatMessage>();

    // Use this for initialization
    void Start()
    {
        mGameControler = GetComponent<GameControler>();
        mButtonControler = GetComponent<ButtonControler>();

        InitComponent();

        InputField inputField = GameObject.Find("InputField").GetComponent<InputField>();
        GameObject.Find("send").GetComponent<Button>().onClick.AddListener(() =>
        {
            string text = string.Format(ChatMessage.Format, mGameControler.selfInfo.Pos, ChatMessage.TEXT, inputField.text);
            mGameHandle.RoomTableChatReq(ByteUtil.ToBytes(text));
            // 清空输入框
            inputField.text = "";
            // 收回聊天视图
            mButtonControler.PopupChatView(false);
        });
    }

    // 添加聊天记录
    public void AddMessage(byte[] body)
    {
        string bodyStr = ByteUtil.ToString(body);
        string[] bodyArray = bodyStr.Split(new char[] { '#' });
        int pos = int.Parse(bodyArray[0]);
        int type = int.Parse(bodyArray[1]);
        string info = bodyArray[2];

        ChatMessage chatM = new ChatMessage();
        chatM.id = mGameControler.tableInfo.Players[pos - 1].Id;
        chatM.type = type;
        chatM.nickname = mGameControler.tableInfo.Players[pos - 1].Nickname;
        chatM.avatar = mGameControler.tableInfo.Players[pos - 1].Avatar;
        chatM.message = info;

        mGameControler.PlayerSay(pos, chatM);
        mChatLogAdapter.AddItem(chatM);
    }

    // 初始化组件
    public void InitComponent()
    {
        chatTypeObj = GameObject.Find("ChatType");
        emojiTypeObj = GameObject.Find("EmojiType");
        chatLogTypeObj = GameObject.Find("ChatLogType");

        chatObj = GameObject.Find("Chat");
        emojiObj = GameObject.Find("Emoji");
        chatLogObj = GameObject.Find("ChatLog");

        mChatAdapter = chatObj.GetComponent<ChatAdapter>();
        mChatAdapter.SetControler(mGameControler.selfInfo.Pos, mButtonControler);
        mEmojiAdapter = emojiObj.GetComponent<EmojiAdapter>();
        mEmojiAdapter.SetControler(mGameControler.selfInfo.Pos, mButtonControler);
        mChatLogAdapter = chatLogObj.GetComponent<ChatLogAdapter>();

        HideAll();
        Show(chatTypeObj, chatObj);

        chatTypeObj.GetComponent<Button>().onClick.AddListener(() =>
        {
            HideAll();
            Show(chatTypeObj, chatObj);
        });
        emojiTypeObj.GetComponent<Button>().onClick.AddListener(() =>
        {
            HideAll();
            Show(emojiTypeObj, emojiObj);
        });
        chatLogTypeObj.GetComponent<Button>().onClick.AddListener(() =>
        {
            HideAll();
            Show(chatLogTypeObj, chatLogObj);
        });
    }

    private void HideAll()
    {
        chatTypeObj.GetComponent<BtnSelectControler>().Select = false;
        emojiTypeObj.GetComponent<BtnSelectControler>().Select = false;
        chatLogTypeObj.GetComponent<BtnSelectControler>().Select = false;
        chatObj.SetActive(false);
        emojiObj.SetActive(false);
        chatLogObj.SetActive(false);
    }

    private void Show(GameObject goType, GameObject goContent)
    {
        goType.GetComponent<BtnSelectControler>().Select = true;
        goContent.SetActive(true);
    }



}
