using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Coordinate
{
    // 6人场
    private static Vector3[] playerPos6 =
    {
        new Vector3(425,345,0),     //0
        new Vector3(820,70,0),      //1
        new Vector3(520,-265,0),    //2
        new Vector3(0,-265,0),      //3
        new Vector3(-520,-265,0),   //4
        new Vector3(-820,70,0),     //5
    };

    private static Vector3[] chipPos6 =
    {
        new Vector3(350,175,0),    //0
        new Vector3(600,60,0),     //1
        new Vector3(300,-150,0),   //2
        new Vector3(-210,-150,0),  //3
        new Vector3(-500,-85,0),   //4
        new Vector3(-600,60,0),    //5
    };

    private static Vector3[] dealerPos6 =
    {
        new Vector3(280,325,0),      //0
        new Vector3(675,-20,0),      //1
        new Vector3(370,-270,0),     //2
        new Vector3(-150,-270,0),    //3
        new Vector3(-370,-270,0),    //4
        new Vector3(-675,-20,0),     //5
    };

    // 9人场
    private static Vector3[] playerPos9 =
    {
        new Vector3(425,345,0),     //0
        new Vector3(780,210,0),     //1
        new Vector3(820,-120,0),    //2
        new Vector3(520,-265,0),    //3
        new Vector3(0,-265,0),      //4
        new Vector3(-520,-265,0),   //5
        new Vector3(-820,-120,0),   //6
        new Vector3(-780,210,0),    //7
        new Vector3(-425,345,0)     //8
    };

    private static Vector3[] chipPos9 =
    {
        new Vector3(350,175,0),     //0
        new Vector3(570,95,0),      //1
        new Vector3(600,-15,0),     //2
        new Vector3(300,-150,0),    //3
        new Vector3(-210,-150,0),   //4
        new Vector3(-500,-85,0),    //5
        new Vector3(-600,5,0),      //6
        new Vector3(-570,95,0),     //7
        new Vector3(-350,175,0)     //8
    };

    private static Vector3[] dealerPos9 =
    {
        new Vector3(280,325,0),      //0
        new Vector3(630,200,0),      //1
        new Vector3(670,-140,0),     //2
        new Vector3(370,-270,0),     //3
        new Vector3(-150,-270,0),    //4
        new Vector3(-370,-270,0),    //5
        new Vector3(-670,-140,0),    //6
        new Vector3(-630,200,0),     //7
        new Vector3(-280,325,0)      //8
    };

    // 获取玩家坐标
    public static Vector3[] GetPlayerPosArray(int count)
    {
        Vector3[] array = null;
        switch (count)
        {
            case 6:
                array = playerPos6;
                break;
            case 9:
                array = playerPos9;
                break;
        }
        return array;
    }

    // 获取筹码坐标
    public static Vector3[] GetChipPosArray(int count)
    {
        Vector3[] array = null;
        switch (count)
        {
            case 6:
                array = chipPos6;
                break;
            case 9:
                array = chipPos9;
                break;
        }
        return array;
    }

    // 获取庄家位置坐标
    public static Vector3[] GetDealerPosArray(int count)
    {
        Vector3[] array = null;
        switch (count)
        {
            case 6:
                array = dealerPos6;
                break;
            case 9:
                array = dealerPos9;
                break;
        }
        return array;
    }

    private static float standard_width = 1920f;            //初始宽度  
    private static float standard_height = 1280f;           //初始高度  

    private static Vector3[] ScalePosition(Vector3[] array)
    {
        //屏幕矫正比例
        //float adjustor = 0f;                      
        // 当前设备高度    
        float device_width = Screen.width;
        // 当前设备高度 
        float device_height = Screen.height;
        // 计算宽高比例  
        float bw = standard_width / device_width;
        float bh = standard_height / device_height;
        // 计算矫正比例  
        //if (device_aspect < standard_aspect)
        //{
        //    adjustor = standard_aspect / device_aspect;
        //}
        for (int i = 0; i < array.Length; i++)
        {
            Vector3 vect = array[i];
            vect.x /= bw;
            vect.y /= bh;
            //vect.z *= adjustor;
            array[i] = vect;
        }
        return array;
    }

}
