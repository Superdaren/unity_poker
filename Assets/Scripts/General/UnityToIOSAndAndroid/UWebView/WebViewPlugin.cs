using System;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;

public class WebViewPlugin
{
#if UNITY_ANDROID
    private static AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    private static AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
#endif
#if UNITY_IOS
    [DllImport("__Internal")]
    static extern void _openWebView(string name, string url);

    [DllImport("__Internal")]
    static extern void _closeWebView();

    [DllImport("__Internal")]
    static extern void _loadUrl(string url);

    [DllImport("__Internal")]
    static extern void _cleanCache();

    [DllImport("__Internal")]
    static extern void _clearCookie(); 

    [DllImport("__Internal")]
    static extern void _registEventWithName(string name);

	[DllImport("__Internal")]
	static extern void _evaluatingJavaScript(string javaScript);
#endif

    /** iOS要与h5交互要注册的名称 **/
    static string[] reginstNames = { "toast", "showMenuText" };

    // 打开Webview
    public static void OpenWebView(string name, string url)
    {
        string loadUrl = String.IsNullOrEmpty(url) ? "about:blank" : url.Trim();

#if UNITY_ANDROID
        jo.Call("OpenWebView", loadUrl);
#endif

#if UNITY_IOS
        _openWebView(name, loadUrl);

        registEventWithNames(reginstNames);
#endif
    }

    // 关闭Webview
    public static void CloseWebView()
    {
#if UNITY_ANDROID
        jo.Call("CloseWebView");
#endif

#if UNITY_IOS
        _closeWebView();
#endif
    }

    // 加载链接
    public static void LoadUrl(string url)
    {
        string loadUrl = String.IsNullOrEmpty(url) ? "about:blank" : url.Trim();

#if UNITY_ANDROID
        jo.Call("LoadUrl", url);
#endif

#if UNITY_IOS
        _loadUrl(url);
#endif
    }

    // 清除缓存
    public static void CleanCache()
    {
#if UNITY_ANDROID
        jo.Call("CleanCache");
#endif

#if UNITY_IOS
        _cleanCache();
#endif
    }

    // 清除cookie
    public void ClearCookie()
    {
#if UNITY_ANDROID
        jo.Call("ClearCookie");
#endif

#if UNITY_IOS
        _clearCookie();
#endif
    }

    // 加载H5回调函数
    public void LoadCallback(String javaScript)
    {
#if UNITY_ANDROID
        jo.Call("LoadCallback", javaScript);
#endif
    }

    // 加载js代码
    public void LoadJavaScript(String javaScript)
    {
#if UNITY_ANDROID
        jo.Call("LoadJavaScript", javaScript);
#endif

#if UNITY_IOS
        _evaluatingJavaScript(javaScript);
#endif
    }

#if UNITY_IOS
    /**
     * iOS 注册事件
     */
    public static void registEventWithName(string name)
    {
        _registEventWithName(name);
    }

    /**
     * iOS 注册多个事件
     */
    public static void registEventWithNames(string[] names)
    {
        foreach (string name in names)
        {
            _registEventWithName(name);
        }
    }
#endif
}
