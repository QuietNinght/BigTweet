using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour {

    //如果没有设置跟随目标，则目标默认为主摄像机
    public Transform target;
    public Vector2 offest;
    public bool followX = true;
    public bool followY = true;

    void Start()
    {
        if (target == null)
        {
            target = Camera.main.transform;
        }
    }

    void Update()
    {
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
