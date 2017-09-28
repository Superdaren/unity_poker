using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatLogAdapter : MonoBehaviour
{
    public ScrollRect scrollRect;           //ScrollRect  
    public VerticalLayoutGroup group;       //VerticalLayoutGroup  
    private List<ChatMessage> cMessages = new List<ChatMessage>();
    private List<GameObject> chatLogObjs = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        foreach (var chatLogObj in chatLogObjs)
        {
            Vector3 vect = chatLogObj.transform.localPosition;
            vect.z = 0;
            chatLogObj.transform.localPosition = vect;
        }
    }

    //创建Item
    public void AddItem(ChatMessage chatM)
    {
        string itemPath = chatM.type == ChatMessage.TEXT ? "Prefabs/ChatLogTextItem" : "Prefabs/ChatLogImageItem";
        var go = Instantiate(Resources.Load(itemPath)) as GameObject;
        go.SetActive(true);
        go.name = "chatLog" + cMessages.Count;
        go.transform.SetParent(group.transform);
        go.transform.localScale = new Vector3(1, 1, 1);
        go.GetComponent<ChatLogItemControler>().InitData(chatM);

        Vector3 vect = go.transform.localPosition;
        vect.z = 0;
        go.transform.localPosition = vect;

        cMessages.Add(chatM);
        chatLogObjs.Add(go);
    }
}
