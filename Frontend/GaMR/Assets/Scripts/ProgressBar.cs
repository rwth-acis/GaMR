using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour {

    public float targetHeight = 0.28f;

    private Vector3 velocity = Vector3.zero;
    public float smoothTime = 0.3f;

    private float progress = 0f;

    public float Progress
    {
        get { return progress; }
        set { progress = value; }
    }

    private void Start()
    {
        transform.localScale = new Vector3(
            transform.localScale.x,
            progress * targetHeight,
            transform.localScale.z);
    }

    private void Update()
    {
        Vector3 targetScale = new Vector3(
            transform.localScale.x,
            progress * targetHeight,
            transform.localScale.z);
        transform.localScale = Vector3.SmoothDamp(transform.localScale, targetScale, ref velocity, smoothTime);
    }
}
