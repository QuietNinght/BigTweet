using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartTest : MonoBehaviour {

    void Awake()
    {
        Debug.Log("调用一次Awake函数");
    }

	// Use this for initialization
	void Start () {
        Debug.Log("调用一次Start函数");	
	}

    void OnEnable()
    {
        Debug.Log("调用一次OnEnable函数");
    }
}
