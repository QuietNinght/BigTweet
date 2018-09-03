using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************************************
 * 控制主角的技能攻击开启时间
 ******************************************************/
public class Skill : MonoBehaviour {

    Player player;
    float duration;

    void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    public void Open(float _duration)
    {
        duration = _duration;
        StartCoroutine(SkillCor());
    }

    IEnumerator SkillCor()
    {
        yield return new WaitForSeconds(duration);
        player.CloseSkill();
    }
	
}
