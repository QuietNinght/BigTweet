﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************************************
 * 存放部分用于存储的字符串
 ******************************************************/
public class GlobalData {

    //音乐，音效 开关
    public static string MusicSwitch = "MusicSwitch";        //背景音乐开关
    public static string SoundSwitch = "SoundSwitch";        //音效开关
    public static string ShockSwitch = "ShockSwitch";        //震动开关

    //最高分
    public static string HighestScore = "HighestScore";

    //金币
    public static string Coin = "Coin";

    //当前出战角色
    public static string FightPlayer = "FightPlayer";
    //角色是否已经解锁（通过该字符串结合角色id进行存取判断）
    public static string PlayerLocked = "PlayerLocked";
    //角色台词
    public static string[] PlayerSlogan = new string[]
    {
        "啾",
        "啾啾",
        "啾啾啾",
        "啾啾啾啾",
        "啾啾啾啾啾",
        "啾啾啾啾啾啾"
    };
    //角色等级
    public static string PlayerLevel = "PlayerLevel";

    //物品生成概率数组 列表，随时间增长，难度级别升高，概率发生变化
    public static List<int[]> GoodsRateListByLevel = new List<int[]>()
    {
        new int[]{ 40, 30, 30 },
        new int[]{ 30, 40, 30 },
        new int[]{ 20, 50, 30 },
        new int[]{ 20, 60, 20 }
    };
    //障碍生成概率数组 列表
    public static List<int[]> BarrierRateListByLevel = new List<int[]>()
    {
        new int[]{ 50, 30, 10, 7, 3 },
        new int[]{ 40, 20, 24, 10,6 },
        new int[]{ 18, 15, 13, 30, 24}
    };
    //道具生成概率数组 列表
    public static List<int[]> PropRateListByLevel = new List<int[]>()
    {
        new int[]{ 12, 12, 12, 14, 50},
        new int[]{ 11, 11, 14, 14, 50},
        new int[]{ 10, 8, 11, 21, 50 }
    };
    //栏杆生成概率数组 列表
    public static List<int[]> RailRateListByLevel = new List<int[]>()
    {
        new int[]{ 50, 30, 15, 3, 2 }
    };
}
