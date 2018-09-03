using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialog : MonoBehaviour {

    public GameObject dialogPanel;

    public Text content;
    public GameObject button1;              //显示确定按钮
    public GameObject button2;              //显示确定，取消按钮
    public GameObject button3;              //显示免费金币，充值按钮

	public static void ShowDialog(string _content, int _type)
    {

    }

    public void Close()
    {
        Destroy(gameObject);
    }
}
