using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************************************
 * 道具 - 姜饼饼干，增加主角元件
 ******************************************************/
public class PropCooky : Prop {

    //数字显示文本
    //private PropNum propNum;

    public override void Init()
    {
        base.Init();
        /*
        //生成数字显示文本，并初始化
        propNum = GameManager.Instance.CreatePropNum(transform, Vector3.zero, point);
        propNum.SetColor(Color.red);
        propNum.SetSize(50);
        */
    }

    public override void OnDestroy()
    {
        //销毁自身
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (player != null)
        {
            Debug.Log("吃到饼干，增加生命");

            //增加主角的元件（现在每个饼干只增加一个元件
            player.AddCell(1);

            OnColliderPlayer();
        }
    }
}
