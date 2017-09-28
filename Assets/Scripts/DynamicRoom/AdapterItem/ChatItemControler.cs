using UnityEngine.UI;

public class ChatItemControler : BaseAdapterItem<string> {

    private int pos;
    private ButtonControler mButtonControler;

    public Text contentText;
    public Button contentButton;

    public override void SetData(string data, params object[] objs)
    {
        if (objs != null)
        {
            pos = (int)objs[0];
            mButtonControler = (ButtonControler)objs[1];
        }
        contentText.text = data;
        contentButton.onClick.RemoveAllListeners();
        contentButton.onClick.AddListener(() =>
        {
            string text = string.Format(ChatMessage.Format, pos, ChatMessage.TEXT, data);
            mButtonControler.mGameHandle.RoomTableChatReq(ByteUtil.ToBytes(text));
            mButtonControler.PopupChatView(false);
        });
    }

}
