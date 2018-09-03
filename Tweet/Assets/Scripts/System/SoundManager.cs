using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    private static SoundManager instance;
    public static SoundManager Instance
    {
        get { return instance; }
    }

    //点击音效
    public AudioClip soundClick;

    //用于播发BGM的声音组件
    private AudioSource musicAudio;
    //用于播发音效的声音组件
    private AudioSource soundAudio;

    void Awake()
    {
        instance = this;

        musicAudio = gameObject.AddComponent<AudioSource>();
        musicAudio.loop = true;

        soundAudio = gameObject.AddComponent<AudioSource>();

        //0表示开，1表示关
        if(PlayerPrefs.GetInt(GlobalData.MusicSwitch,0) == 0)
        {
            musicAudio.enabled = true;
        }
        else if(PlayerPrefs.GetInt(GlobalData.MusicSwitch,0) == 1)
        {
            musicAudio.enabled = false;
        }

        if(PlayerPrefs.GetInt(GlobalData.SoundSwitch,0) == 0)
        {
            soundAudio.enabled = true;
        }
        else if(PlayerPrefs.GetInt(GlobalData.SoundSwitch,0) == 1)
        {
            soundAudio.enabled = false;
        }
    }

    //音乐开关
    public static void MusicSwitch()
    {
        if (Instance.musicAudio.enabled)
        {
            //关
            Instance.musicAudio.enabled = false;
            PlayerPrefs.SetInt(GlobalData.MusicSwitch, 1);
        }
        else
        {
            //开
            Instance.musicAudio.enabled = true;
            PlayerPrefs.SetInt(GlobalData.MusicSwitch, 0);
        }
    }
    //音效开关
    public static void SoundSwitch()
    {
        if (Instance.soundAudio.enabled)
        {
            //关
            Instance.soundAudio.enabled = false;
            PlayerPrefs.SetInt(GlobalData.SoundSwitch, 1);
        }
        else
        {
            //开
            Instance.soundAudio.enabled = true;
            PlayerPrefs.SetInt(GlobalData.SoundSwitch, 0);
        }
    }

    //播发音效
    public static void PlaySound(AudioClip clip)
    {
        Instance.PlayVoice(clip, Instance.soundAudio);
    }
    public static void PlaySound(AudioClip clip, float volume)
    {
        Instance.PlayVoice(clip, Instance.soundAudio, volume);
    }

    //播发音乐
    public static void PlayMusic(AudioClip clip)
    {
        Instance.PlayVoice(clip, Instance.musicAudio);
    }
    public static void PlayMusic(AudioClip clip,float volume)
    {
        Instance.PlayVoice(clip, Instance.musicAudio, volume);
    }

    //播发声音
    private void PlayVoice(AudioClip clip, AudioSource audioOut)
    {
        if(clip == null)
        {
            Debug.LogError("-------------- 未设置音乐文件 --------------");
            return;
        }

        if(audioOut == musicAudio)
        {
            audioOut.clip = clip;
            audioOut.Play();
        }
        else
        {
            audioOut.PlayOneShot(clip);
        }
    }

    private void PlayVoice(AudioClip clip, AudioSource audioOut, float volume)
    {
        if (clip == null)
        {
            Debug.LogError("-------------- 未设置音乐文件 --------------");
            return;
        }

        audioOut.volume = volume;

        if (audioOut == musicAudio)
        {
            audioOut.clip = clip;
            audioOut.Play();
        }
        else
        {
            audioOut.PlayOneShot(clip);
        }
    }
}
