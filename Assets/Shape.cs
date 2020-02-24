using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Shape : MonoBehaviour
{
//    private Vector2 size = new Vector2(5, 5);
    public bool selectMe;
    public float size = 0.05f;
    public int shapeId;
    private void Awake()
    {
//        transform.position = new Vector3(Random.Range(-size.x,size.y),0,Random.Range(-size.x,size.y));

    }

    private void OnValidate()
    {
        selectMe = true;
    }

    private void FixedUpdate()
    {
        size = transform.localScale.magnitude*0.03f;
        if (selectMe)
        {
            Paint.Instance.RenderAllExceptThisToPrevTex(transform);
            Paint.Instance.RenderSingle(transform.position,shapeId);

            selectMe = false;
        }
        if (transform.hasChanged)
        {
            print("The transform has changed!");
            transform.hasChanged = false;
            Paint.Instance.RenderSingle(transform.position,shapeId);

        }
    }


}
