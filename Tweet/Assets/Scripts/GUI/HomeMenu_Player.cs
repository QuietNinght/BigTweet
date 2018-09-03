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
        public Sprite attackIcon;
        public string attackInfo;
    }
    public AttackData[] PlayerAttackDataList;

    private int fightPlayerID;                  //当前出战角色的id
    private int lookedPlayerID;                 //当前查看的角色id
    private int currentCoinNum;                 //当前玩家获得的金币

    [Header("Charactor Value")]
    public Text playerIntroduceText;            //主角介绍文本
    public Image playerBackgoundImg;            //主角背景图片
    public Image attackIconImg;                 //主角攻击类型图标
    public Text attackInfoText;                 //主角攻击类型介绍文本

    [Header("UI Setting")]
    public RectTransform PlayerListRoot;        //放置角色 图片/对象 的根物体
    public int howManyPlayer = 5;               //记录一共有多少个可选角色
    public HomeMenu_PlayerInfo[] playerList;    //可选角色列表

    //一次移动的变化量
    private float step = 720f;
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


    void Start ()
    {
        currentCoinNum = PlayerPrefs.GetInt(GlobalData.Coin, 0);
        RefreshCoinShow(currentCoinNum);

        //初始化显示出战角色
        fightPlayerID = PlayerPrefs.GetInt(GlobalData.FightPlayer, 0);
        PlayerListRoot.anchoredPosition = new Vector2((fightPlayerID * -step), PlayerListRoot.anchoredPosition.y);

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
        if(PlayerPrefs.GetInt(GlobalData.Coin, 0) >= playerList[lookedPlayerID].price)
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
        playerIntroduceText.text = playerList[lookedPlayerID].introduce;        //角色介绍
        playerBackgoundImg.sprite = playerList[lookedPlayerID].backGround;      //角色背景图片

        var index = (int)playerList[lookedPlayerID].attackType;                 //角色攻击类型编号
        attackIconImg.sprite = PlayerAttackDataList[index].attackIcon;          //角色攻击类型图标
        attackInfoText.text = PlayerAttackDataList[index].attackInfo;           //角色攻击类型信息

        //0表示未解锁，1表示解锁
        if(PlayerPrefs.GetInt(GlobalData.PlayerLocked + lookedPlayerID, 0) == 0)
        {
            //显示购买按钮
            RefreshBtnShow(0);
            playerPriceText.text = playerList[lookedPlayerID].price.ToString();
        }
        else if(PlayerPrefs.GetInt(GlobalData.PlayerLocked + lookedPlayerID, 0) == 1)
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

    //刷新按钮显示
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
            newPosX -= step;

            if(newPosX < -step * (howManyPlayer - 1))
            {
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
            newPosX += step;

            //判断要进行的位移是否超出边界
            if(newPosX > 0)
            {
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
        PlayerPrefs.SetInt(GlobalData.FightPlayer, lookedPlayerID);
    }

    //点击购买按钮
    public void OnClickBuy()
    {
        //判断金币是否足够
        if (HaveEnoughMoney())
        {
            SureBuyPanel.SetActive(true);
        }
        else
        {
            NotEnoughPanel.SetActive(true);
        }
    }

    //确认购买
    public void OnSureBuy()
    {
        //刷新按钮显示：显示可出战按钮
        RefreshBtnShow(1);
        //更新玩家金币
        var newCoin = currentCoinNum - playerList[lookedPlayerID].price;
        RefreshCoinShow(newCoin);
        PlayerPrefs.SetInt(GlobalData.Coin, newCoin);
        //更新角色 是否解锁 存档数据
        PlayerPrefs.SetInt(GlobalData.PlayerLocked + lookedPlayerID, 1);
    }

    //关闭弹窗
    public void Close()
    {
        SureBuyPanel.SetActive(false);
        NotEnoughPanel.SetActive(false);
    }
}
