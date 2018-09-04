using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu_GameUI : MonoBehaviour {

    public Text scoreText;                  //分数文本
    public Transform energyGroundSprite;    //能量条显示能量的图片

    public Player player;

    void Start()
    {
        player = FindObjectOfType<Player>();
    }

	void Update () {
        //刷新得分
        scoreText.text = GameManager.Instance.Score.ToString();
        //刷新能量条
        float energyPercent = player.Energy / player.maxEnergy;
        energyGroundSprite.localScale = new Vector3(1, energyPercent, 1);
	}
}
