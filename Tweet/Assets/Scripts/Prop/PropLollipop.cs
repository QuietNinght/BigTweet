using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************************************
 * 道具 - 棒棒糖，开启主角的攻击
 ******************************************************/
public class PropLollipop : Prop {

    public float duration;

    public void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (player != null)
        {
            Debug.Log("吃到棒棒糖，开启技能攻击");

            //开启主角攻击功能
            player.OpenSkill(duration);

            OnColliderPlayer();
        }
    }

}
