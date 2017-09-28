using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmojiAdapter : MonoBehaviour
{
    private int pos;
    private ButtonControler mButtonControler;
    public void SetControler(int pos, ButtonControler mButtonControler)
    {
        this.pos = pos;
        this.mButtonControler = mButtonControler;
        CreateItem();                    //创建Item  
    }

    public ScrollRect scrollRect;           //ScrollRect  
    public GridLayoutGroup group;           //VerticalLayoutGroup  
    public GameObject prefab_item;          //预设               
    private int maxCount = 30;              //生产 数量    

    void Start()
    {

    }

    //创建Item  
    void CreateItem()
    {
        for (int i = 0; i < maxCount; i++)
        {
            var go = Instantiate(prefab_item) as GameObject;
            go.SetActive(true);
            go.name = "emoji" + i;
            go.transform.SetParent(group.transform);
            go.transform.localScale = new Vector3(1, 1, 1);
            go.GetComponent<Image>().sprite = Resources.LoadAll<Sprite>("Textures/room/gameChat")[i];
            string message = "gameChat_" + i;
            go.GetComponent<Button>().onClick.AddListener(() =>
            {
                string text = string.Format(ChatMessage.Format, pos, ChatMessage.EMOJI, message);
                mButtonControler.mGameHandle.RoomTableChatReq(ByteUtil.ToBytes(text));
                mButtonControler.PopupChatView(false);
            });
        }
    }

}
