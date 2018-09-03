using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************************************
 * 磁铁，控制其开启关闭，以及吸取道具功能
 ******************************************************/
public class Magnet : MonoBehaviour {

    Player player;

    CircleCollider2D magnetCollider;

    float duration;

    void Awake()
    {
        player = GetComponentInParent<Player>();
        magnetCollider = GetComponent<CircleCollider2D>();
        magnetCollider.enabled = false;
    }

    public void Open(float _duration)
    {
        gameObject.SetActive(true);
        magnetCollider.enabled = true;
        duration = _duration;
        StartCoroutine(MagnetCor());
    }

    public void Close()
    {
        gameObject.SetActive(false);
        magnetCollider.enabled = false;
        StopAllCoroutines();
    }

    IEnumerator MagnetCor()
    {
        yield return new WaitForSeconds(duration);
        Close();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Prop"))
        {
            other.gameObject.GetComponent<PropMove>().enabled = true;
        }
    }
}
