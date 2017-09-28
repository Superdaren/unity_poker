using UnityEngine;
using System.Collections.Generic;

public class UnityIOSAndroid
{
	/**
     * 转换来自iOS and Andriod的消息
     */
    public static Dictionary<string, string> parseMsg(string msg)
	{
		if (null == msg || 0 == msg.Length)
		{
            return null;
		}

		Dictionary<string, string> dicMsg = new Dictionary<string, string>();
		string[] msgArray = msg.Split('&');
		for (int i = 0; i < msgArray.Length; i++)
		{
			string[] elementarray = msgArray[i].Split('=');
			dicMsg.Add(elementarray[0], elementarray[1]);
		}

        return dicMsg;
	}
}
