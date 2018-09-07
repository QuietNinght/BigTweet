using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/******************************************************
 * 游戏管理器
 ******************************************************/
public class GameManager : MonoBehaviour {

    private static GameManager instance;
    public static GameManager Instance
    {
        get { return instance; }
    }

    [Header("Proporty")]
    //private GameObject mGameObject;
    private Tweener currentScoreAni;

    public GameObject MapRoot;

    [Header("Player")]
    public Vector3 basePos;
    public Transform playerBody;

    [Header("Prefab")]
    public GameObject propNumPrefab;
    public GameObject[] playerPrefabArr;

    public GameObject initMapPrefab;

    public GameObject bodersPrefab;

    [Header("Effect")]
    public AudioClip gameBg;

    [Header("Content")]
    [HideInInspector]
    public Player player;
    public GameObject mCamera;

    public enum GameState
    {
        Menu,
        Game,
        Pause,
        Finish
    }
    public GameState gameState;

	void Awake()
    {
        instance = this;
        //mGameObject = gameObject;
    }

    //得分
    public int Score { get; set; }

    //增加分数
    public void AddScore(int _add)
    {
        if(_add <= 0)
        {
            return;
        }

        if (_add == 1)
        {
            Score += _add;
        }
        else
        {
            IncrementPoint(Score + _add);
        }
    }

    //递增改变分数
    void IncrementPoint(int _targetSocre)
    {
        if (currentScoreAni != null)
        {
            currentScoreAni.Kill();
        }
        currentScoreAni = DOTween.To(() => Score, r => Score = r, _targetSocre, 1f).OnComplete(() =>
        {
            Score = _targetSocre;
        });
    }

    //整个游戏场景的入口
    void Start()
    {
        GameStart();
    }

    //游戏开始
    public void GameStart()
    {
        gameState = GameState.Game;

        /************************************* 初始化游戏内容 *************************************/

        //生成初始地图，并初始化
        InitSpawn initMap = Instantiate(initMapPrefab, MapRoot.transform).GetComponent<InitSpawn>();
        initMap.Init();

        //生成主角，根据出战角色
        int playerID = PlayerPrefs.GetInt(GlobalData.FightPlayer, 0);
        Transform p = Instantiate(playerPrefabArr[playerID], playerBody).transform;
        p.localPosition = basePos;

        player = p.GetComponent<Player>();
        player.Init();

        //初始化界面
        MenuManager.Instance.Init(player);

        //初始化地图管理器
        MapManager.Instance.Init();

        //初始化物品生成器
        SpawnManager.Instance.Init(player);

        //初始化相机跟随功能
        mCamera.GetComponent<CameraMove>().Init(player);
        player.isPlaying = true;

        //生成游戏边界
        Instantiate(bodersPrefab, Vector3.zero, Quaternion.identity);

        /************************************* 初始化游戏内容完毕 *************************************/

        //播发音乐
        //SoundManager.PlayMusic(gameBg);
    }
    //游戏结束
    public void GameFinish()
    {
        //判断本局得分是否超过最高分
        if (Score > PlayerPrefs.GetInt(GlobalData.HighestScore, 0))
        {
            PlayerPrefs.SetInt(GlobalData.HighestScore, Score);
        }

        MenuManager.Instance.GameFinish();
    }

    //生成一个跟随文本，显示目标要显示的数据
    public PropNum CreatePropNum(Transform _target, Vector3 _offest, int _num)
    {
        //生成一个数字文本并初始化
        var propNum = Instantiate(propNumPrefab).GetComponent<PropNum>();
        propNum.Init(_target, _offest, _num);

        return propNum;
    }
}
