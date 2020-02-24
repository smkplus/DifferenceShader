using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlaneResizer : MonoBehaviour
{
    private void Start()
    {
        Camera cam = Camera.main;

        float pos = (cam.nearClipPlane + 10.0f);

        transform.position = cam.transform.position + cam.transform.forward * pos;
        transform.LookAt (cam.transform);
        transform.Rotate (90.0f, 0.0f, 0.0f);

        float h = (Mathf.Tan(cam.fieldOfView*Mathf.Deg2Rad*0.5f)*pos*2f) /10.0f;

        transform.localScale = new Vector3(h*cam.aspect,1.0f, h);
    }
}
