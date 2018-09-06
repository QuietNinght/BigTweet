using UnityEngine;
using UnityEngine.EventSystems;

public class HomeMenu_Player_AttackType : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public GameObject attackTypeIntro;

    void Start()
    {
        attackTypeIntro.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //鼠标点击对象，按下鼠标时对象响应此事件
        attackTypeIntro.SetActive(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //鼠标点击对象，抬起鼠标时响应
        //无论鼠标在何处抬起（即不在对象中）都会在对象中响应此事件
        //注：响应此事件的前提是A对象必须响应过OnPointerDown事件
        attackTypeIntro.SetActive(false);
    }
}
