using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAdaptation : MonoBehaviour {

    public float devWidth = 7.5f;
    public float devHeight = 13.35f;

    public Camera controlCamera;

    private float orthSize;
    private float sizeRatio;
    void Update()
    {
        orthSize = controlCamera.orthographicSize;
        sizeRatio = (float)Screen.width / Screen.height;
        float width = orthSize * 2 * sizeRatio;
        
        if(width != devWidth)
        {
            float height = devWidth / sizeRatio;
            controlCamera.orthographicSize = height / 2;
        }
    }
}
