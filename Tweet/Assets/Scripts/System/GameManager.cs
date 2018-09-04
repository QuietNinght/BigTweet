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
    //public float maxEnergy;
    //public float Energy { get; set; }
    //public float sprintDuration;
    //public float sprintSpeed;
    //public float energyClearTime;
    //private bool isEnergyGrouping = true;

    [Header("Player")]
    public Vector3 basePos;
    public Transform playerBody;

    [Header("Prefab")]
    public GameObject propNumPrefab;
    public GameObject[] playerPrefabArr;

    [Header("Effect")]
    public AudioClip gameBg;

    [Header("Content")]
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
    }

    //得分
    public int Score { get; set; }

    public void AddScore(int _add)
    {
        Score += _add;
    }

    //测试时使用，生成器实现后删除该函数
    void Start()
    {
        GameStart();
    }

    void Update()
    {

    }

    //游戏开始
    public void GameStart()
    {
        gameState = GameState.Game;
        //初始化界面
        MenuManager.Instance.GameStart();

        //初始化游戏内容
        //生成主角，根据出战角色
        /*int playerID = PlayerPrefs.GetInt(GlobalData.FightPlayer, 0);
        Transform p = Instantiate(playerPrefabArr[playerID], basePos, Quaternion.identity).transform;
        p.SetParent(playerBody);
        player = p.GetComponent<Player>();*/

        //开启相机跟随
        mCamera.GetComponent<CameraMove>().isFollowing = true;
        player.isPlaying = true;

        //初始化游戏内容完毕

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
