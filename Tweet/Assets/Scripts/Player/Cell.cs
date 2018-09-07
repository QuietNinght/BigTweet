using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************************************
 * 主角身后跟随的元件
 ******************************************************/
public class Cell : MonoBehaviour {

    //跟随目标，每个元件跟随其前一个元件移动，第一个元件跟随主角
    private Transform previous;
    //是否移动
    private bool isMove;
    //与前一个元件的距离
    private Vector2 offest;

    private float velocityXSmoothing;
    //水平加速时间
    public float accelerationTime = 0.05f;
    //死亡时使用的材质
    public Material deadMaterial;
    //死亡特效
    public GameObject deadEffect;

    private MoveController controller;
    private Vector2 velocity;
    private SpriteRenderer render;
    private BoxCollider2D bCollider;
    private Animator anim;

    void Awake()
    {
        controller = GetComponent<MoveController>();
        render = GetComponentInChildren<SpriteRenderer>();
        bCollider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    public void Init(Transform _previous, Vector2 _space)
    {
        previous = _previous;
        offest = _space;
        isMove = true;
    }

    public void Refresh(Transform _previous, Vector2 _offest)
    {
        previous = _previous;
        offest = _offest;
    }

    //目标，限制元件的移动，在遇到border和障碍的时候停止移动
    void Update() {
        
        if (isMove)
        {
            //var endPos = new Vector2(previous.position.x, previous.position.y - space);

            var newPosX = previous.position.x + offest.x;
            //在没有碰到阻碍的时候，才能进行左右移动
            velocity.x = Mathf.SmoothDamp(transform.position.x, newPosX, ref velocityXSmoothing, accelerationTime);
            velocity.y = previous.position.y + offest.y;
            //进行跟随移动
            //transform.position = new Vector2(newPosX, newPosY);
        }
        
    }

    void LateUpdate()
    {
        if (isMove)
        {
            controller.Move(velocity);
        }
    }

    public void DestroyCell()
    {
        isMove = false;
        anim.enabled = false;
        bCollider.enabled = false;

        render.material = deadMaterial;
        if(deadEffect != null)
        {
            Instantiate(deadEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject, 0.1f);
    }
}
