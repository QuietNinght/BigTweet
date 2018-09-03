using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************************************
 * 道具基类
 ******************************************************/
public class Prop : MonoBehaviour {

    //被拾取音效
    public AudioClip pickAudio;
    //销毁特效
    public GameObject destroyEffect;
    //自身Transform组件
    public Transform mTransform;
    //主角
    [HideInInspector]
    public Player player;

    //初始化函数
    public virtual void Init()
    {
        mTransform = transform;
    }

    public virtual void Update()
    {

    }

    public virtual void OnColliderPlayer()
    {
        //生成特效
        if (destroyEffect != null)
        {
            Instantiate(destroyEffect, mTransform.position, Quaternion.identity);
        }

        //被拾取音效
        if (pickAudio != null)
        {
            SoundManager.PlaySound(pickAudio);
        }

        //销毁道具
        OnDestroy();
    }

    public virtual void OnColliderMagent()
    {
        
    }

    public virtual void OnDestroy()
    {
        //销毁自身
        Destroy(gameObject);
    }
}
