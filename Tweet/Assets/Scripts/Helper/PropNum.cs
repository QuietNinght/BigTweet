using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/******************************************************
 * 道具上显示的数字
 ******************************************************/
public class PropNum : MonoBehaviour {

    //文本的父物体
    private Transform root;
    //跟随对象
    private Transform target;
    //相对跟随对象的位置偏移
    private Vector3 offest;
    //是否移动
    private bool isMove = false;
    //文本组件
    private Text numText;

    void Awake()
    {
        root = GameObject.Find("PropNumRoots").transform;
    }

    public void Init(Transform _target, Vector3 _offest, int _num)
    {
        //绑定父物体
        transform.SetParent(root);
        //绑定跟随目标
        target = _target;
        //记录位置偏移
        offest = _offest;
        //显示数字
        numText = GetComponent<Text>();
        numText.text = _num.ToString();
        //开启移动
        isMove = true;
    }

    void Update()
    {
        if (isMove)
        {
            if(target == null)
            {
                //Debug.Log("------------- 数字文本的目标对象消失，销毁数字文本 -------------");
                Destroy(gameObject);
            }
            else
            {
                //将目标在世界中的坐标转换为屏幕坐标
                var targetPos = Camera.main.WorldToScreenPoint(target.position);
                //移动该文本到计算出来的坐标
                transform.position = targetPos + offest;
            }
        }
    }

    //刷新文本显示
    public void RefreshNum(int _num)
    {
        if(numText == null)
        {
            Debug.Log("------------- 未初始化数字的文本组件，现在重新赋值 -------------");
            numText = GetComponent<Text>();
        }
        numText.text = _num.ToString();
    }

    //设置文本颜色
    public void SetColor(Color color)
    {
        if (numText == null)
        {
            Debug.Log("------------- 未初始化数字的文本组件，现在重新赋值 -------------");
            numText = GetComponent<Text>();
        }
        numText.color = color;
    }

    //设置文本字体大小
    public void SetSize(int size)
    {
        if (numText == null)
        {
            Debug.Log("------------- 未初始化数字的文本组件，现在重新赋值 -------------");
            numText = GetComponent<Text>();
        }
        numText.fontSize = size;
    }
}
