using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropMove : MonoBehaviour {
    public float speed = 10f;

    private Player player;

    void Awake()
    {
        enabled = false;
    }

    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
    }
}
