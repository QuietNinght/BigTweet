using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotEnoughCoinsPanel : MonoBehaviour {

    public Text freeTimeText;
    public float freeTime;

    private bool canGetFree;

    void Start()
    {
        canGetFree = false;
        freeTimeText.text = ((int)freeTime).ToString();
    }

    void Update()
    {
        if(canGetFree == false)
        {
            freeTime -= Time.deltaTime;
            if (freeTime <= 0)
            {
                canGetFree = true;
                freeTimeText.text = "Free";
                return;
            }
            freeTimeText.text = ((int)freeTime).ToString();
        }
    }

    public void OnClickClose()
    {
        gameObject.SetActive(false);
    }

    public void OnClickFreeCoin()
    {
        if (canGetFree)
        {
            //获得免费金币
        }
    }

    public void OnClickCharge()
    {

    }
	
}
