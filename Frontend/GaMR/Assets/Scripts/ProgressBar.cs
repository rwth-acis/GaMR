﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour {

    [Tooltip("The full height of the progress bar")]
    public float targetHeight = 0.28f;

    private Vector3 velocity = Vector3.zero;
    [Tooltip("The time needed to complete the movement to the new value")]
    public float smoothTime = 0.3f;
    [SerializeField]
    private GameObject progressBar;
    [SerializeField]
    private GameObject hull;
    [SerializeField]
    private GameObject upperCap;
    private float badgeHeight = 0.25f;

    [SerializeField]
    private float progress = 0f;

    /// <summary>
    /// The progress which is currently displayed by the progress bar
    /// </summary>
    public float Progress
    {
        get { return progress; }
        set { progress = value; }
    }

    /// <summary>
    /// sets the progress bar to 0 without smoothing
    /// places the badge
    /// </summary>
    private void Start()
    {
        progressBar.transform.localScale = new Vector3(
            progressBar.transform.localScale.x,
            progress * targetHeight,
            progressBar.transform.localScale.z);

        hull.transform.localScale = new Vector3(
            hull.transform.localScale.x,
            targetHeight,
            hull.transform.localScale.z);

        upperCap.transform.localPosition = new Vector3(
            upperCap.transform.localPosition.x,
            targetHeight * 2 + 0.05f,
            upperCap.transform.localPosition.z);

        GameObject badge = (GameObject)Instantiate(Resources.Load("Badge"));
        Bounds badgeBounds = Geometry.GetBoundsIndependentFromRotation(badge.transform);
        badge.transform.parent = transform;
        badge.transform.localPosition = upperCap.transform.localPosition + new Vector3(0, badgeBounds.size.x + 0.25f, 0);
    }

    /// <summary>
    /// smoothly moves the progress bar to match the current progress value
    /// </summary>
    private void Update()
    {
        progress = Math.Min(Math.Max(0, progress), 1);

        Vector3 targetScale = new Vector3(
            progressBar.transform.localScale.x,
            progress * targetHeight,
            progressBar.transform.localScale.z);
        progressBar.transform.localScale = Vector3.SmoothDamp(progressBar.transform.localScale, targetScale, ref velocity, smoothTime);
    }
}