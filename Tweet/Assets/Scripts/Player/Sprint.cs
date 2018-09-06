using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************************************
 * 护盾，控制其开启关闭
 ******************************************************/
public class Sprint : MonoBehaviour {

    Player player;

    float duration;

    void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    public void Open(float _duration)
    {
        gameObject.SetActive(true);

        duration = _duration;

        //先关闭可能存在的冲刺协程
        StopAllCoroutines();
        //开启冲刺
        StartCoroutine(SprintCor());
    }

    public void Close()
    {
        gameObject.SetActive(false);
        StopAllCoroutines();
    }

    IEnumerator SprintCor()
    {
        if (player != null)
        {
            yield return new WaitForSeconds(duration);

            player.CloseSprint();
            Close();
        }
    }
}
