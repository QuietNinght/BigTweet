using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************************************
 * 主角发射的子弹
 ******************************************************/
public class BulletByPlayer : Bullet {

    public int damage;                  //攻击力
    public float liveTime;              //存活时间
    public bool isPlay = true;          //是否激活

    public AudioClip hitSound;          //击中音效
    public GameObject destroyEffect;    //销毁特效

    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if ((liveTime -= Time.deltaTime) <= 0)
        {
            //销毁
            DestroyBullet();
            return;
        }

        if (isPlay)
        {
            //前进
            transform.Translate((Direction + new Vector2(0, InitialVelocity.y)) * Speed * Time.deltaTime, Space.World);
        }
    }

    void DestroyBullet()
    {
        if(destroyEffect != null)
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        //如果碰撞到非指定攻击对象
        if ((LayerCollision.value & (1 << other.gameObject.layer)) == 0)
        {
            OnNotCollideWith(other);
            return;
        }

        //判断碰撞到发射者
        var isOwner = Owner == other.gameObject;
        if (isOwner)
        {
            OnCollideOwner();
            return;
        }

        if (other.GetComponent<Barrier>() != null)
        {
            OnCollideTarget(other);
            return;
        }
        else
        {
            OnCollideOther(other);
            return;
        }
    }

    protected override void OnCollideOther(Collider2D other)
    {
        //SoundManager.PlaySound(hitSound);
        DestroyBullet();
    }

    protected override void OnCollideTarget(Collider2D other)
    {
        var barrier = other.GetComponent<Barrier>();
        if(barrier != null)
        {
            barrier.OnDamage(damage, gameObject);

            //关闭子弹的物理效果
            GetComponent<BoxCollider2D>().enabled = false;
            GetComponent<Rigidbody2D>().simulated = false;

            //SoundManager.PlaySound(hitSound);
            DestroyBullet();
        }
    }
}
