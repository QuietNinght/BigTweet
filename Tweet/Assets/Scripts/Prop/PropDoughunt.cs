using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************************************
 * 道具 - 甜甜圈，护盾 +　爆炸（销毁整行障碍）
 ******************************************************/
public class PropDoughunt : Prop {

    public float duration;

    public void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (player != null)
        {
            Debug.Log("吃到甜甜圈，开启护盾");

            //开启护盾功能
            player.OpenShield(duration);

            OnColliderPlayer();
        }
    }
}
