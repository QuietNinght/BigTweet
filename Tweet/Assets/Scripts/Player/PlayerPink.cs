using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPink : Player {

    [Header("Own Property")]
    public GameObject propLollipopPrefab;
    public GameObject haloExplodeEffect;            //光环爆炸特效
    public float baseHaloChargeTime = 30f;
    private GameObject Halo;

    private float haloTime = 0;

    private float realHaloChargeTime;
    private float passiveEffectIncrement = -5f;
    private bool isOpenHalo = false;                //是否开启了光环
    private bool isChargeHalo = false;              //是否开始光环蓄能

    public override void Init()
    {
        base.Init();
        Halo = mTransform.Find("Halo").gameObject;
    }

    protected override void Update()
    {
        base.Update();

        if (isPlaying)
        {
            ChargeHalo();
        }
    }

    //光环蓄能
    private void ChargeHalo()
    {
        if (isChargeHalo)
        {
            haloTime += Time.deltaTime;
            if(haloTime >= realHaloChargeTime)
            {
                haloTime = 0;
                isOpenHalo = true;
                isChargeHalo = false;

                Halo.SetActive(true);
            }
        }
    }

    //触发光环
    private void TriggerHalo(Vector3 _pos)
    {
        //在被撞击的障碍处生成特效
        Instantiate(haloExplodeEffect, _pos, Quaternion.identity);
        //在被撞击的障碍处生成一个棒棒糖
        Instantiate(propLollipopPrefab, _pos, Quaternion.identity);
        isOpenHalo = false;
        isChargeHalo = true;
        Halo.SetActive(false);
    }

    protected override void Attack(Barrier barrier)
    {
        if (isShield)
        {
            //如果加持了护盾，将一排敌人全部销毁
            Transform line = barrier.transform.parent;
            Barrier[] lineBarriers = line.GetComponentsInChildren<Barrier>();

            foreach (Barrier similar in lineBarriers)
            {
                //安全检测，确认得到的障碍与被撞击的障碍处于同一行
                if (similar.Y == barrier.Y)
                {
                    similar.OnExplode();
                }
            }

            //如果开启了 甜蜜光环   
            if (isOpenHalo)
            {
                TriggerHalo(barrier.transform.position);
            }

            //撞击后减少护盾韧性/关闭护盾
            CloseShield();
        }
        else if (isSprint && isShield == false)
        {
            //如果在冲刺状态，直接撞毁敌人
            barrier.OnExplode();

            //如果开启了 甜蜜光环   
            if (isOpenHalo)
            {
                TriggerHalo(barrier.transform.position);
            }
        }
        else
        {
            //如果开启了 甜蜜光环   
            if (isOpenHalo)
            {
                TriggerHalo(barrier.transform.position);

                barrier.OnExplode();
                return;
            }

            StartCoroutine(AttackCor(barrier));
        }
    }

    protected override void RangeSkill()
    {
        //远程技能：弹射，发射子弹进行攻击
        timer += Time.deltaTime;
        if (timer >= rangeSkillCoolTime)
        {
            timer = 0;
            //在开火点生成一颗子弹
            Bullet bullet = (Bullet)Instantiate(bulletPrefab, firePoint.position, Quaternion.identity).GetComponent(typeof(Bullet));
            bullet.Init(gameObject, Vector2.up, Vector2.up);
        }
    }

    public override void OnRangeHitBarrier(Barrier barrier, int damage, GameObject instigator)
    {
        //如果开启了 甜蜜光环   
        if (isOpenHalo)
        {
            TriggerHalo(barrier.transform.position);

            barrier.OnExplode();
            return;
        }
        barrier.OnDamage(damage, gameObject);
    }

    protected override void PassiveSkill()
    {
        //被动技能，每30s获得一层“甜蜜光环”
        //下次攻击的敌人直接爆炸获得得分，并将其变为棒棒糖
        isChargeHalo = true;
        realHaloChargeTime = baseHaloChargeTime + passiveEffectIncrement * Level;
    }
}
