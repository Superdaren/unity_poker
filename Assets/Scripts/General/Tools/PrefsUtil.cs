using System;
using UnityEngine;

public class PrefsUtil
{
    public const string ServiceId = "service_id";//服务器ID

    public static int GetInt(string key)
    {
        return PlayerPrefs.GetInt(key);
    }

    public static float GetFloat(string key)
    {
        return PlayerPrefs.GetFloat(key);
    }

    public static string GetString(string key)
    {
        return PlayerPrefs.GetString(key);
    }

    public static void Set(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }

    public static void Set(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
    }

    public static void Set(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }

    public static void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }

    public static void Delete(string key)
    {
        PlayerPrefs.DeleteKey(key);
    }

}
