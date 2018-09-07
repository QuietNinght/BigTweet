using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Menu_GameUI : MonoBehaviour {

    public Text scoreText;                  //分数文本
    public Transform energyGroundSprite;    //能量条显示能量的图片
    private float energyGroundSpriteMaxX;

    private Player player;

    void Start()
    {
        player = MenuManager.Instance.player;
        energyGroundSpriteMaxX = energyGroundSprite.GetComponent<RectTransform>().rect.width;
    }

	void Update () {

        //刷新得分
        scoreText.text = GameManager.Instance.Score.ToString();
        //刷新能量条
        float energyPercent = player.Energy / player.maxEnergy;
        //energyGroundSprite.localScale = new Vector3(energyPercent, 1, 1);
        float newPosX = energyGroundSpriteMaxX * energyPercent;
        newPosX = Mathf.Clamp(newPosX, 0, energyGroundSpriteMaxX);
        energyGroundSprite.localPosition = new Vector2(newPosX, energyGroundSprite.localPosition.y);
    }
}
