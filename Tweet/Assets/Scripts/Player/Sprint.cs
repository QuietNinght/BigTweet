using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************************************
 * 护盾，控制其开启关闭
 ******************************************************/
public class Sprint : MonoBehaviour {

    Player player;

    float speed;

    float duration;

    BoxCollider2D sprintCollider;

    void Awake()
    {
        player = GetComponentInParent<Player>();
        sprintCollider = GetComponent<BoxCollider2D>();
        sprintCollider.enabled = false;
    }

    public void Open(float _duration, float _speed)
    {
        gameObject.SetActive(true);
        sprintCollider.enabled = true;

        speed = _speed;
        duration = _duration;

        //先关闭可能存在的冲刺协程
        StopAllCoroutines();
        //开启冲刺
        StartCoroutine(SprintCor());
    }

    public void Close()
    {
        gameObject.SetActive(false);
        sprintCollider.enabled = false;
        StopAllCoroutines();
    }

    IEnumerator SprintCor()
    {
        if (player != null)
        {
            //设置主角速度为冲刺速度
            player.SetSpeed(speed);
            //开启无敌状态
            player.OpenGodMode();

            yield return new WaitForSeconds(duration);

            //重置主角速度
            player.ResetSpeed();
            //关闭无敌状态
            player.CloseGodMode();
            //重置主角的碰撞
            player.ResetCollision();

            Close();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var barrier = other.GetComponent<Barrier>();
        if(barrier != null)
        {
            //只能销毁前方的障碍
            barrier.OnExplode();
        }
    }
}
