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
    private float space;

    private float velocityXSmoothing;
    //水平加速时间
    public float accelerationTime = 0.05f;

    private MoveController controller;
    private Vector2 velocity;

    public void Init(Transform _previous, float _space)
    {
        previous = _previous;
        space = _space;
        isMove = true;
        controller = GetComponent<MoveController>();
    }

    public void Refresh(Transform _previous, float _space)
    {
        previous = _previous;
        space = _space;
    }

    //目标，限制元件的移动，在遇到border和障碍的时候停止移动
    void Update() {
        
        if (isMove)
        {
            //var endPos = new Vector2(previous.position.x, previous.position.y - space);

            var newPosX = previous.position.x;
            //在没有碰到阻碍的时候，才能进行左右移动
            velocity.x = Mathf.SmoothDamp(transform.position.x, newPosX, ref velocityXSmoothing, accelerationTime);
            velocity.y = previous.position.y - space;
            //进行跟随移动
            //transform.position = new Vector2(newPosX, newPosY);
        }
        
    }

    void LateUpdate()
    {
        controller.Move(velocity);
    }

    public void Move(Vector3 velocity)
    {
        
    }
}
