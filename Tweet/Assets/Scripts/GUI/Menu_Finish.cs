using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu_Finish : MonoBehaviour {

    public Text highScoreText;
    public Text nowScoreText;

	void Start () {
        //显示最高分
        highScoreText.text = PlayerPrefs.GetInt(GlobalData.HighestScore, 0).ToString();
        //显示本局得分
        nowScoreText.text = GameManager.Instance.Score.ToString();
	}

}
