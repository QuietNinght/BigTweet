using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/******************************************************
 * 主界面
 ******************************************************/
public class HomeMenu_Start : MonoBehaviour {
    //最高分
    public Text highScoreText;
    //出战角色
    public Image fightPlayerImg;
    //出战角色台词
    public Text playerSlogan;
    //角色图片数组
    public Sprite[] playerSpriteArr;
	void Start ()
    {
        highScoreText.text = PlayerPrefs.GetInt(GlobalData.HighestScore, 0).ToString();
    }
	
    void OnEnable()
    {
        var playerNum = PlayerPrefs.GetInt(GlobalData.FightPlayer, 0);
        fightPlayerImg.sprite = playerSpriteArr[playerNum];
        fightPlayerImg.SetNativeSize();
        playerSlogan.text = GlobalData.PlayerSlogan[playerNum];
    }
}
