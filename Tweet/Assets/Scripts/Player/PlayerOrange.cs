using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOrange : Player
{

    public int basePassiveEffect = 5;

    private int passiveEffectIncrement = 5;
    private int realPassiveEffect;

    public override void OpenShield(float _duration)
    {
        base.OpenShield(_duration);
        //增加得分
        GameManager.Instance.AddScore(realPassiveEffect);
    }

    protected override void MeleeSkill(Barrier victim)
    {
        //近战技能：连击，二次攻击敌人
        victim.OnDamage(damage, gameObject);
    }

    protected override void PassiveSkill()
    {
        //食用甜甜圈+5分
        realPassiveEffect = basePassiveEffect + passiveEffectIncrement * Level;
    }
}
