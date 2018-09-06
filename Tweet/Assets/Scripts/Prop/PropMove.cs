using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropMove : MonoBehaviour {
    public float speed = 10f;

    private Player player;

    private SpriteRenderer render;
    private Vector3 offest;

    void Awake()
    {
        enabled = false;
        render = GetComponentInChildren<SpriteRenderer>();

        offest = new Vector3(0, render.size.y, 0);
    }

    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    void Update()
    {
        Vector3 newPos = player.mTransform.position - offest;
        transform.position = Vector3.MoveTowards(transform.position, newPos, speed * Time.deltaTime);
    }
}
