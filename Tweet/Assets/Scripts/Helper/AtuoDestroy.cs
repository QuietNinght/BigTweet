using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtuoDestroy : MonoBehaviour {

    public enum DetroyType
    {
        ByTime,
        ByPlayer,
    }
    public DetroyType type;

    public float time;

    //主角
    Player player;
    Transform mTransform;

    void Awake()
    {
        player = FindObjectOfType<Player>();
        mTransform = transform;
    }

    void Update () {
        if(type == DetroyType.ByTime)
        {
            time -= Time.deltaTime;
            if (time <= 0)
            {
                Destroy(gameObject);
            }
        }
        else if(type == DetroyType.ByPlayer)
        {
            if ((player.mTransform.position.y - mTransform.position.y) > 10)
            {
                Destroy(gameObject);
            }
        }
	}
}
