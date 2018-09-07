using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChangeShowOnClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{

    //角色图片数组
    public Sprite[] playerSpriteArr;
    //角色被点击的图片数组
    public Sprite[] playerClickSpriteArr;
    public Image playerSprite;

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        int playerNum = PlayerPrefs.GetInt(GlobalData.FightPlayer, 0);
        playerSprite.sprite = playerClickSpriteArr[playerNum];
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        int playerNum = PlayerPrefs.GetInt(GlobalData.FightPlayer, 0);
        playerSprite.sprite = playerSpriteArr[playerNum];
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        int playerNum = PlayerPrefs.GetInt(GlobalData.FightPlayer, 0);
        playerSprite.sprite = playerSpriteArr[playerNum];
    }
}
