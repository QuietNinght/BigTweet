using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/******************************************************
 * 角色选择界面
 ******************************************************/
public class HomeMenu_Player : MonoBehaviour {

    [Header("Game Value")]
    public Text coinText;                       //金币文本

    [System.Serializable]
    public struct AttackData                    //攻击类型对应信息结构体
    {
        public HomeMenu_PlayerInfo.AttackType attackType;
        public Sprite attackIcon;
        public string attackInfo;
    }
    public AttackData[] AttackDataArr;

    private Dictionary<HomeMenu_PlayerInfo.AttackType, AttackData> AttackTypeDataDic;

    private int fightPlayerID;                  //当前出战角色的id
    private int lookedPlayerID;                 //当前查看的角色id
    private int currentCoinNum;                 //当前玩家获得的金币

    [Header("Charactor Value")]
    public Text playerNameText;                 //主角姓名文本
    public Text playerIntroduceText;            //主角介绍文本
    public Image playerBackgoundImg;            //主角背景图片
    public Image attackIconImg;                 //主角攻击类型图标
    public Text attackIntroText;                //主角攻击类型介绍文本

    [Header("UI Setting")]
    public RectTransform PlayerListRoot;        //放置角色 图片/对象 的根物体
    public int howManyPlayer = 5;               //记录一共有多少个可选角色
    public HomeMenu_PlayerInfo[] playerList;    //可选角色列表

    //一次移动的变化量
    public float onceStep = 210f;
    private bool sliding = false;
    private float smooth = 10f;
    private float newPosX = 0;

    [Header("Button")]
    public GameObject buyBtn;                   //购买按钮
    public GameObject fightBtn;                 //可出战按钮
    public GameObject readyBtn;                 //已出战显示按钮
    public Text playerPriceText;                //角色价格文本

    [Header("Panel")]
    public GameObject NotEnoughPanel;           //金币不够的提示弹窗
    public GameObject SureBuyPanel;             //确认购买的提示弹窗


    void Awake ()
    {
        //测试用，先给自己100金币
        PlayerPrefs.SetInt(GlobalData.Coin, 100);

        AttackTypeDataDic = new Dictionary<HomeMenu_PlayerInfo.AttackType, AttackData>();
        for(int i = 0; i < AttackDataArr.Length; i++)
        {
            if(AttackTypeDataDic.ContainsKey(AttackDataArr[i].attackType) == false)
            {
                AttackTypeDataDic.Add(AttackDataArr[i].attackType, AttackDataArr[i]);
            }
        }

        howManyPlayer = playerList.Length;
    }

    void OnEnable()
    {
        currentCoinNum = PlayerPrefs.GetInt(GlobalData.Coin, 0);
        RefreshCoinShow(currentCoinNum);

        //初始化显示出战角色
        fightPlayerID = PlayerPrefs.GetInt(GlobalData.FightPlayer, 0);
        Debug.Log("当前出战角色id:" + fightPlayerID);
        if (PlayerPrefs.GetInt(GlobalData.PlayerLocked + 0, 0) == 0)
        {
            //第一个角色默认为解锁状态
            PlayerPrefs.SetInt(GlobalData.PlayerLocked + 0, 1);
        }
        //移动显示出战角色
        newPosX = fightPlayerID * -onceStep;
        //sliding = true;
        PlayerListRoot.anchoredPosition = new Vector2(fightPlayerID * -onceStep, PlayerListRoot.anchoredPosition.y);

        //初始化选中角色信息
        lookedPlayerID = fightPlayerID;
        RefreshPlayerInfoShow();
    }
	
	void Update ()
    {
        if (sliding)
        {
            float X = Mathf.Lerp(PlayerListRoot.anchoredPosition.x, newPosX, smooth * Time.deltaTime);
            PlayerListRoot.anchoredPosition = new Vector2(X, PlayerListRoot.anchoredPosition.y);

            if (Mathf.Abs(PlayerListRoot.anchoredPosition.x - newPosX) < 10)
            {
                PlayerListRoot.anchoredPosition = new Vector2(newPosX, PlayerListRoot.anchoredPosition.y);
                sliding = false;

                RefreshPlayerInfoShow();
            }
        }
	}

    //判断金币是否足够
    bool HaveEnoughMoney()
    {
        Debug.Log("当前拥有的金币数：" + currentCoinNum);
        Debug.Log("角色的售价：" + playerList[lookedPlayerID].price);
        if(currentCoinNum >= playerList[lookedPlayerID].price)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //刷新角色信息显示
    void RefreshPlayerInfoShow()
    {
        Debug.Log("当前切换到的角色id：" + lookedPlayerID);

        playerNameText.text = playerList[lookedPlayerID].playerName;           //角色名字
        playerIntroduceText.text = playerList[lookedPlayerID].introduce;       //角色介绍
        playerBackgoundImg.sprite = playerList[lookedPlayerID].backGround;     //角色背景图片

        var type = playerList[lookedPlayerID].attackType;                      //角色攻击类型编号
        attackIconImg.sprite = AttackTypeDataDic[type].attackIcon;             //角色攻击类型图标
        attackIntroText.text = AttackTypeDataDic[type].attackInfo;             //角色攻击类型信息

        int isLocked = PlayerPrefs.GetInt(GlobalData.PlayerLocked +lookedPlayerID, 0);
        Debug.Log("角色是否已经解锁：" + isLocked);
        //0表示未解锁，1表示解锁
        if (isLocked == 0)
        {
            //显示购买按钮
            RefreshBtnShow(0);
            playerPriceText.text = playerList[lookedPlayerID].price.ToString();
        }
        else if(isLocked == 1)
        {
            if (lookedPlayerID == fightPlayerID)
            {
                //显示已出战按钮
                RefreshBtnShow(2);
            }
            else
            {
                //显示可出战按钮
                RefreshBtnShow(1);
            }
        }
    }

    //刷新按钮显示（0：显示购买按钮； 1：显示出战按钮； 2：显示已出战按钮
    void RefreshBtnShow(int type)
    {
        switch (type)
        {
            case 0:
                buyBtn.SetActive(true);
                fightBtn.SetActive(false);
                readyBtn.SetActive(false);
                break;
            case 1:
                buyBtn.SetActive(false);
                fightBtn.SetActive(true);
                readyBtn.SetActive(false);
                break;
            case 2:
                buyBtn.SetActive(false);
                fightBtn.SetActive(false);
                readyBtn.SetActive(true);
                break;
        }
    }

    void RefreshCoinShow(int num)
    {
        coinText.text = num.ToString();
    }

    /*****************************************
     * 按钮功能
     ******************************************/
     //下一个角色
    public void OnClickNextPlayer()
    {
        if (!sliding)
        {
            if(lookedPlayerID == playerList.Length - 1)
            {
                return;
            }

            newPosX -= onceStep;

            if(newPosX < -onceStep * (howManyPlayer - 1))
            {
                newPosX = -onceStep * (howManyPlayer - 1);
                sliding = true;
                return;
            }
            else
            {
                //newPosX = Mathf.Clamp(newPosX, -step * (howManyPlayer - 1), 0);
                sliding = true;
                lookedPlayerID++;
            }
        }
    }

    //上一个角色
    public void OnClickPrevPlayer()
    {
        if (!sliding)
        {
            if(lookedPlayerID == 0)
            {
                return;
            }

            newPosX += onceStep;

            //判断要进行的位移是否超出边界
            if(newPosX > 0)
            {
                newPosX = 0;
                sliding = true;
                return;
            }
            else
            {
                //newPosX = Mathf.Clamp(newPosX, -step * (howManyPlayer - 1), 0);
                sliding = true;
                lookedPlayerID--;
            }
        }
    }

    //出战
    public void OnClickFight()
    {
        //更新出战角色id
        fightPlayerID = lookedPlayerID;
        //更新出战角色id存档
        PlayerPrefs.SetInt(GlobalData.FightPlayer, fightPlayerID);
        //刷新按钮显示
        RefreshBtnShow(2);
    }

    //点击购买按钮
    public void OnClickBuy()
    {
        SureBuyPanel.SetActive(true);
    }

    //确认购买
    public void OnSureBuy()
    {
        //判断金币是否足够
        if (HaveEnoughMoney())
        {
            //刷新按钮显示：显示可出战按钮
            RefreshBtnShow(1);
            //更新玩家金币
            var newCoin = currentCoinNum - playerList[lookedPlayerID].price;
            RefreshCoinShow(newCoin);
            PlayerPrefs.SetInt(GlobalData.Coin, newCoin);
            currentCoinNum = newCoin;
            //更新角色 是否解锁 存档数据
            PlayerPrefs.SetInt(GlobalData.PlayerLocked + lookedPlayerID, 1);
            Debug.Log("解锁的角色id为：" + lookedPlayerID);
            //关闭确认购买面板
            SureBuyPanel.SetActive(false);
        }
        else
        {
            //关闭确认购买面板
            SureBuyPanel.SetActive(false);
            NotEnoughPanel.SetActive(true);
        }
    }

    //取消购买
    public void OnCancelBuy()
    {
        SureBuyPanel.SetActive(false);
    }

    //关闭弹窗
    public void Close()
    {
        SureBuyPanel.SetActive(false);
        NotEnoughPanel.SetActive(false);
    }
}
