using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlue : Player
{
    private float passiveEffectIncrement = 0.05f;

    protected override void MeleeSkill(Barrier victim)
    {
        //近战技能：横扫，同时攻击对象的左右障碍
        Transform line = victim.transform.parent;
        Barrier[] lineBarriers = line.GetComponentsInChildren<Barrier>();
        foreach (Barrier implicate in lineBarriers)
        {
            //安全检测，确认得到的障碍与被撞击的障碍处于同一行
            if (implicate.Y == victim.Y)
            {
                //判断得到的障碍与被撞击的障碍是否相差一格或两格（这是基于当前4根跑道的操作，如果跑道数量变化，则不可用
                var dist = Mathf.Abs(implicate.X - victim.X);
                if (dist == 1 || dist == 2)
                {
                    //调用该障碍的被伤害函数
                    implicate.OnDamage(damage, gameObject);
                }
            }
        }
        if(meleeAttackEffect != null)
        {
            //播放特效
            Instantiate(meleeAttackEffect, victim.transform.position, Quaternion.identity);
        }
    }

    protected override void PassiveSkill()
    {
        //被动技能，增加磁铁持续时间20%
        magentDuartionRate = 0.2f + passiveEffectIncrement * Level;
    }
}
