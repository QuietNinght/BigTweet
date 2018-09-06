using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyGameObjectOnTrigger : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            return;
        }

        Destroy(other.gameObject);
    }
}
