using System;
using UnityEngine;
using UnityEngine.UI;

public class ChatLogItemControler : MonoBehaviour
{

    public Image avatar;
    public Text nickname;
    public Image contentImage;
    public Text contentText;

    // 初始化数据
    public void InitData(ChatMessage chatM)
    {
        try
        {
            avatar.SetWebImage(chatM.avatar, "");
            nickname.text = string.IsNullOrEmpty(chatM.avatar) ? chatM.id.ToString() : chatM.avatar;
            if (contentImage != null)
            {
                int index = int.Parse(chatM.message.Split('_')[1]);
                contentImage.sprite = Resources.LoadAll<Sprite>("Textures/room/gameChat")[index];
            }
            else
            {
                contentText.text = chatM.message;
            }
        }
        catch (Exception e)
        {
            print(e.Message);
        }
    }
}
