using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour {

    //如果没有设置跟随目标，则目标默认为主摄像机
    public Transform target;
    public Vector2 offest;
    public bool followX = true;
    public bool followY = true;

    private Transform borderLeft;
    private Transform borderRight;
    private float limit, preLimit;
    public float correctX = 0.15f;

    void Start()
    {
        if (target == null)
        {
            target = Camera.main.transform;
        }

        borderLeft = transform.Find("Left");
        borderRight = transform.Find("Right");
        preLimit = limit = Camera.main.orthographicSize * Screen.width / Screen.height;
        borderLeft.position = new Vector3(-limit + correctX, borderLeft.position.y);
        borderRight.position = new Vector3(limit - correctX, borderRight.position.y); 
    }

    void Update()
    {
        limit = Camera.main.orthographicSize * Screen.width / Screen.height;
        if(limit != preLimit)
        {
            //更新左右限制框的位置
            borderLeft.position = new Vector3(-limit, borderLeft.position.y);
            borderRight.position = new Vector3(limit, borderRight.position.y);
            preLimit = limit;
        }

        Vector3 follow = target.position + (Vector3)offest;

        if (followX && followY)
        {
            transform.position = new Vector3(follow.x, follow.y, transform.position.z);
        }
        else if (followX)
        {
            transform.position = new Vector3(follow.x, transform.position.y, transform.position.z);
        }
        else if (followY)
        {
            transform.position = new Vector3(transform.position.x, follow.y, transform.position.z);
        }
    }
}
