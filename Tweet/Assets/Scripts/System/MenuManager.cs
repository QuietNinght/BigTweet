using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/******************************************************
 * 界面管理器
 ******************************************************/
public class MenuManager : MonoBehaviour {

    private static MenuManager instance;
    public static MenuManager Instance
    {
        get { return instance; }
    }

    //游戏场景中的各界面
    public GameObject Menu_GameUI;
    public GameObject Menu_Start;
    public GameObject Menu_Pause;
    public GameObject Menu_Finish;
    public GameObject Menu_Loading;

	void Awake()
    {
        instance = this;

        Menu_Start.SetActive(true);
        Menu_GameUI.SetActive(false);
        Menu_Pause.SetActive(false);
        Menu_Finish.SetActive(false);
        Menu_Loading.SetActive(false);
    }

    public void GameStart()
    {
        Menu_Start.SetActive(false);
        Menu_GameUI.SetActive(true);
    }

    public void GameFinish()
    {
        Menu_GameUI.SetActive(false);
        Menu_Finish.SetActive(true);
    }

    ///<summary>
    ///点击事件
    /// </summary>
    //暂停
    public void OnClickPause()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.PlaySound(SoundManager.Instance.soundClick);
        }

        //如果在运行状态，则暂停
        if(Time.timeScale == 1)
        {
            Menu_Pause.SetActive(true);
            Menu_GameUI.SetActive(false);
            //游戏时间比例为0，即停止运行
            Time.timeScale = 0;
        }
    }
    //结束暂停，继续游戏
    public void OnClickContinue()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.PlaySound(SoundManager.Instance.soundClick);
        }

        //如果已经暂停，则恢复运行
        if (Time.timeScale == 0)
        {
            Menu_Pause.SetActive(false);
            Menu_GameUI.SetActive(true);
            Time.timeScale = 1;
        }
    }
    //返回主界面
    public void OnClickHome()
    {
        Time.timeScale = 1;
        SoundManager.PlaySound(SoundManager.Instance.soundClick);
        //LoadingScreen.Show();
        SceneManager.LoadSceneAsync("Home");
        
    }
    //重新开始
    public void OnClickRestart()
    {
        Time.timeScale = 1;
        //SoundManager.PlaySound(SoundManager.Instance.soundClick);
        //播放过渡场景
        //LoadingScreen.Show();
        //SceneManager.GetActiveScene().buildIndex:获取当前活动的场景，然后LoadSceneAsync加载
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
}
