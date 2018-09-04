using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeMenu_Set : MonoBehaviour {

    private GameObject musicSelectImg;
    private GameObject musicUnselectImg;

    private GameObject soundSelectImg;
    private GameObject soundUnselectImg;

    private GameObject shockSelectImg;
    private GameObject shockUnselectImg;
	
    void Awake()
    {
        Transform mTransform = transform;
        musicSelectImg = mTransform.Find("Panel/MusicSwitch/BtnSwitch/select").gameObject;
        musicUnselectImg = mTransform.Find("Panel/MusicSwitch/BtnSwitch/unselect").gameObject;

        soundSelectImg = mTransform.Find("Panel/SoundSwitch/BtnSwitch/select").gameObject;
        soundUnselectImg = mTransform.Find("Panel/SoundSwitch/BtnSwitch/unselect").gameObject;

        shockSelectImg = mTransform.Find("Panel/ShockSwitch/BtnSwitch/select").gameObject;
        shockUnselectImg = mTransform.Find("Panel/ShockSwitch/BtnSwitch/unselect").gameObject;
    }

	void Start () {
		//检测各设置的开关情况；0 表示开，1 表示关
        if(PlayerPrefs.GetInt(GlobalData.MusicSwitch,0) == 0)
        {
            musicSelectImg.SetActive(true);
            musicUnselectImg.SetActive(false);
        }
        else if (PlayerPrefs.GetInt(GlobalData.MusicSwitch, 0) == 1)
        {
            musicSelectImg.SetActive(false);
            musicUnselectImg.SetActive(true);
        }

        if (PlayerPrefs.GetInt(GlobalData.SoundSwitch, 0) == 0)
        {
            soundSelectImg.SetActive(true);
            soundUnselectImg.SetActive(false);
        }
        else if (PlayerPrefs.GetInt(GlobalData.SoundSwitch, 0) == 1)
        {
            soundSelectImg.SetActive(false);
            soundUnselectImg.SetActive(true);
        }

        if (PlayerPrefs.GetInt(GlobalData.ShockSwitch, 0) == 0)
        {
            shockSelectImg.SetActive(true);
            shockUnselectImg.SetActive(false);
        }
        else if (PlayerPrefs.GetInt(GlobalData.ShockSwitch, 0) == 1)
        {
            shockSelectImg.SetActive(false);
            shockUnselectImg.SetActive(true);
        }
    }
	
	public void MusicSwitch()
    {
        if (PlayerPrefs.GetInt(GlobalData.MusicSwitch, 0) == 0)
        {
            musicSelectImg.SetActive(false);
            musicUnselectImg.SetActive(true);
        }
        else if (PlayerPrefs.GetInt(GlobalData.MusicSwitch, 0) == 1)
        {
            musicSelectImg.SetActive(true);
            musicUnselectImg.SetActive(false);
        }
        SoundManager.MusicSwitch();
    }

    public void SoundSwitch()
    {
        if (PlayerPrefs.GetInt(GlobalData.SoundSwitch, 0) == 0)
        {
            soundSelectImg.SetActive(false);
            soundUnselectImg.SetActive(true);
        }
        else if (PlayerPrefs.GetInt(GlobalData.SoundSwitch, 0) == 1)
        {
            soundSelectImg.SetActive(true);
            soundUnselectImg.SetActive(false);
        }
        SoundManager.SoundSwitch();
    }

    public void ShockSwitch()
    {
        if (PlayerPrefs.GetInt(GlobalData.ShockSwitch, 0) == 0)
        {
            shockSelectImg.SetActive(false);
            shockUnselectImg.SetActive(true);
            PlayerPrefs.SetInt(GlobalData.ShockSwitch, 1);
        }
        else if (PlayerPrefs.GetInt(GlobalData.ShockSwitch, 0) == 1)
        {
            shockSelectImg.SetActive(true);
            shockUnselectImg.SetActive(false);
            PlayerPrefs.SetInt(GlobalData.ShockSwitch, 0);
        }
    }
}
