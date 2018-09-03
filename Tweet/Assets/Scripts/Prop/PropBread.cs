using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************************************
 * 道具 - U型面包，开启磁铁功能
 ******************************************************/
public class PropBread : Prop {

    public float duration;

    public void OnTriggerEnter2D(Collider2D other)
    {
        player = other.GetComponent<Player>();
        if (player != null)
        {
            Debug.Log("吃到面包，开启磁铁");

            //开启磁铁功能
            player.OpenMagnet(duration);

            OnColliderPlayer();
        }
    }
}
