using NetProto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameUtil : Singleton<FrameUtil>
{
    //游戏的FPS，可在属性窗口中修改
    public int targetFrameRate = 60;

    //当程序唤醒时
    void Awake()
    {
        //修改当前的FPS
        Application.targetFrameRate = targetFrameRate;
    }
}
