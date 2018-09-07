using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************************************
 * 护盾，控制其开启关闭，以及碰到障碍进行爆炸功能
 ******************************************************/
public class Shield : MonoBehaviour {

    Player player;

    float duration;

    public int tenacity;        //护盾的韧性

    void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    public void Open(float _duration)
    {
        gameObject.SetActive(true);
        duration = _duration;
        //目前的设定是：护盾只能通过碰撞障碍物消耗
        //StartCoroutine(ShieldCor());
    }

    public void WeakenShield(int _strength)
    {
        tenacity -= _strength;
        if (tenacity <= 0)
        {
            Close();
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
        StopAllCoroutines();
    }

    IEnumerator ShieldCor()
    {
        yield return new WaitForSeconds(duration);
        player.CloseShield();
    }
}
