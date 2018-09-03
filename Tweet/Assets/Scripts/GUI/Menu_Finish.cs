using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu_Finish : MonoBehaviour {

    public Text highScoreText;

	void Start () {
        //显示最高分
        highScoreText.text = PlayerPrefs.GetInt(GlobalData.HighestScore, 0).ToString();
	}

}
