﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************************************
 * 角色选择界面里不同角色绑定的脚本
 ******************************************************/
public class HomeMenu_PlayerInfo : MonoBehaviour
{

    public string playerName;
    public int playerID;
    public int price;
    public string introduce;
    public Sprite backGround;

    public enum AttackType
    {
        None,
        Melee,
        Range,
        All
    }
    public AttackType attackType;
}