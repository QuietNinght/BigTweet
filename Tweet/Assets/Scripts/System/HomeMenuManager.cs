using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeMenuManager : MonoBehaviour {

    private static HomeMenuManager instance;
    public static HomeMenuManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new HomeMenuManager();
            }
            return instance;
        }
    }

    //UI界面
    public GameObject HomeMenu_Start;
    public GameObject HomeMenu_Set;
    public GameObject HomeMenu_Rank;
    public GameObject HomeMenu_Store;
    public GameObject HomeMenu_Player;
    public GameObject HomeMenu_Loading;

    void Start()
    {
        HomeMenu_Start.SetActive(true);
        HomeMenu_Set.SetActive(false);
        HomeMenu_Rank.SetActive(false);
        HomeMenu_Store.SetActive(false);
        HomeMenu_Player.SetActive(false);
        HomeMenu_Loading.SetActive(false);
    }

    public void OpenSetUI()
    {
        HomeMenu_Start.SetActive(false);
        HomeMenu_Set.SetActive(true);
    }

    public void OpenRankUI()
    {
        HomeMenu_Start.SetActive(false);
        HomeMenu_Rank.SetActive(true);
    }

    public void OpenStoreUI()
    {
        HomeMenu_Start.SetActive(false);
        HomeMenu_Store.SetActive(true);
    }

    public void OpenPlayerUI()
    {
        HomeMenu_Start.SetActive(false);
        HomeMenu_Player.SetActive(true);
    }

    public void OpenStartUI()
    {
        HomeMenu_Start.SetActive(true);
        HomeMenu_Set.SetActive(false);
        HomeMenu_Rank.SetActive(false);
        HomeMenu_Store.SetActive(false);
        HomeMenu_Player.SetActive(false);
    }
}
