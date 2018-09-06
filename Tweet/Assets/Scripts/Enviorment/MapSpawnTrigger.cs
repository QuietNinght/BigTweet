using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSpawnTrigger : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if(player != null)
        {
            MapManager.Instance.CreateMap();
            Destroy(gameObject);
        }
    }
}
