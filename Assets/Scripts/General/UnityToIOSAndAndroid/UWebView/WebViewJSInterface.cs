using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebViewJSInterface : MonoBehaviour
{
	public void webViewCallBack(string msg)
	{
		Debug.Log("webViewCallBack----------" + msg);
	}

    public void Toast(string content)
    {
        print("+++++++++++++++++++++++++++Toast=" + content);
        PopUtil.ShowMessageBoxWithConfirm("Title",content);
    }
}
