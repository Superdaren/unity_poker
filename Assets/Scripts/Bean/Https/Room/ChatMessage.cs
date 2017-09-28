using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ChatMessage
{
    //pos#type#body----座位号#类型#消息体
    public const string Format = "{0}#{1}#{2}";

    public const int TEXT = 1;
    public const int EMOJI = 2;

    public Int32 id;
    public int type;//1-文字 2-表情
    public string nickname;
    public string avatar;
    public string message;
}
