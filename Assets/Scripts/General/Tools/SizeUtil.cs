using UnityEngine;
using System.Collections;

public class SizeUtil
{
    /**
     * 屏幕分辨率的宽度
     */
    public static float ResolutionWidth
    {
        get
        {
            return Screen.resolutions[0].width;
        }
    }

    /**
     * 屏幕分辨率的高度
     */
    public static float ResolutionHeight
    {
        get
        {
            return Screen.resolutions[0].height;
        }
    }

    /**
     * 频幕的宽度
     */
    public static float ScreenWidth
    {
        get
        {
            return Screen.width;
        }
    }

    /**
     * 频幕的高度
     */
    public static float ScreenHeight
    {
        get
        {
            return Screen.height;
        }
    }
}
