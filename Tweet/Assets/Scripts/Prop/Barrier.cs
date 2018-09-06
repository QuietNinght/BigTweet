using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************************************
 * 场景中的障碍
 ******************************************************/
public class Barrier : MonoBehaviour {
    //障碍的图片渲染器
    public SpriteRenderer render;
    //绑定在该障碍上的数字显示文本对象
    private PropNum propNum;
    [HideInInspector]
    //障碍可被撞击次数
    public int point;
    //每次撞击为主角添加的分数
    public int scoreToAdd;
    //被主角撞击的间隔时间
    public float hitSpaceTime;
    //数字文本生成位置
    public Transform numPonint;

    public int X { get; set; }
    public int Y { get; set; }

    //初始化障碍：x，y坐标，图片，分数
	public void Init(int _x, int _y, Sprite _sprite,int _point)
    {
        X = _x;
        Y = _y;
        render.sprite = _sprite;
        point = _point;
        //生成数字显示文本
        propNum = GameManager.Instance.CreatePropNum(numPonint, Vector3.zero, point);
        GetComponentInChildren<SpriteRenderer>().sortingOrder = -_y;
    }

    /**最开始的主角障碍撞击检测及功能执行，暂时注释
    void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.gameObject.GetComponent<Player>();
        //如果碰撞对象是主角
        if(player != null)
        {
            if (player.OpenCollision())
            {
                //开启碰撞辅助协程，用于刷新数字显示，检查是否销毁
                StartCoroutine(CollisionPlayerCor(player));
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        var player = other.gameObject.GetComponent<Player>();
        //如果碰撞对象是主角
        if (player != null)
        {
            if(player.isHitting == false)
            {
                if (player.OpenCollision())
                {
                    //开启碰撞辅助协程，用于刷新数字显示，检查是否销毁
                    StartCoroutine(CollisionPlayerCor(player));
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        var player = other.gameObject.GetComponent<Player>();
        //如果主角离开
        if (player != null)
        {
            player.CloseCollision();
            //关闭所有协程
            StopAllCoroutines();
        }
    }

    //碰撞辅助协程
    IEnumerator CollisionPlayerCor(Player player)
    {
        //减少主角的元件(如果返回false，说明主角没有元件了，直接结束游戏
        if (player.RemoveCell())
        {
            //增加得分
            GameManager.Instance.AddScore(scoreToAdd);
            //减少可被撞击次数
            point--;
            //如果point<=0，则销毁障碍
            if (point <= 0)
            {
                OnDestory();
            }
            //刷新数字文本
            propNum.RefreshNum(point);
            //生成特效

            yield return new WaitForSeconds(hitSpaceTime);

            //如果point>0，则继续调用该协程
            if (point > 0)
            {
                StartCoroutine(CollisionPlayerCor(player));
            }
        }      
    }
    **/

    //销毁障碍
    void OnDestory()
    {
        //关闭所有协程
        //StopAllCoroutines();
        //同时销毁在UI上的数字文本
        //Destroy(propNum.gameObject);

        //死亡特效

        Destroy(gameObject);
    }

    //受到伤害
    public void OnDamage(int damage, GameObject instigator)
    {
        //增加得分
        GameManager.Instance.AddScore(scoreToAdd);
        //减少可被撞击次数
        point -= damage;
        //如果point<=0，则销毁障碍
        if (point <= 0)
        {
            OnDestory();
            return;
        }
        else
        {
            //刷新数字文本
            propNum.RefreshNum(point);
            //生成特效

        }
    }

    //炸毁障碍
    public void OnExplode()
    {
        //增加得分
        GameManager.Instance.AddScore(point * scoreToAdd);

        OnDestory();
    }
}
