using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************************************
 * 子弹基类
 ******************************************************/
public class Bullet : MonoBehaviour {

    //速度
    public float Speed;
    //子弹攻击对象
    public LayerMask LayerCollision;

    //发射子弹的对象
    public GameObject Owner { get; private set; }
    //子弹方向
    public Vector2 Direction { get; private set; }
    //子弹初始速度（二维向量）
    public Vector2 InitialVelocity { get; private set; }

    //初始化子弹属性
    public void Init(GameObject owner, Vector2 direction, Vector2 initialVelocity)
    {
        transform.up = direction;               //修正子弹朝向
        Owner = owner;
        Direction = direction;
        InitialVelocity = initialVelocity;
    }

    //不同的子弹需要对该函数进行重写
    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        //如果碰撞到非指定攻击对象
        if ((LayerCollision.value & (1 << other.gameObject.layer)) == 0)
        {
            Debug.Log("碰到非指定攻击对象");
            OnNotCollideWith(other);
            return;
        }

        //判断碰撞到发射者
        var isOwner = Owner == other.gameObject;
        if (isOwner)
        {
            Debug.Log("碰到发射者");
            OnCollideOwner();
            return;
        }
    }

    //碰撞到非攻击对象时，调用该方法
    protected virtual void OnNotCollideWith(Collider2D other) { }

    //碰撞到发射子弹者时，调用该方法
    protected virtual void OnCollideOwner() { }

    //碰撞到攻击对象时，调用该方法造成伤害
    protected virtual void OnCollideTarget(Collider2D other) { }

    //碰撞到以上三种情况外的其它东西时，调用该方法进行处理
    protected virtual void OnCollideOther(Collider2D other) { }
}
