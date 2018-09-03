using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************************************
 * 道具 - 巧克力，冲刺
 ******************************************************/
public class PropChocolate : Prop {

    //冲刺的持续时间
    public float duration;
    //冲刺速度
    public float sprintSpeed;

    void OnTriggerEnter2D(Collider2D other)
    {
        player = other.GetComponent<Player>();
        if (player != null)
        {
            Debug.Log("吃到巧克力，开启冲击");

            player.OpenSprint(duration, sprintSpeed);

            OnColliderPlayer();
        }
    }
}
