using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FadeInOnFocus : MonoBehaviour, IFocusable
{
    public float fadeDuration = 1f;
    public float minimumAlpha = 0.4f;
    private Material mat;
    private float alpha = 1f;
    private bool inFocus;
    private bool matInitializedInLastFrame = false;

    // Use this for initialization
    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    public void OnFocusEnter()
    {
        if (mat != null)
        {
            StopAllCoroutines();
            StartCoroutine(Fade(true, fadeDuration));
        }
        inFocus = true;
    }

    public void OnFocusExit()
    {
        if (mat != null)
        {
            StopAllCoroutines();
            StartCoroutine(Fade(false, fadeDuration));
        }
        inFocus = false;
    }

    public void Update()
    {
        // when the annotation is created, the user can immediately look away
        // sometimes the material was not yet instantiated at that point
        // and so the annotation does not fade out
        // this makes sure that this is corrected as soon as the material is available
        if (mat != null && !matInitializedInLastFrame)
        {
            matInitializedInLastFrame = true;
            StartCoroutine(Fade(inFocus, fadeDuration));
        }
    }

    private IEnumerator Fade(bool fadeIn, float duration)
    {
        float dir;
        if (fadeIn)
        {
            dir = minimumAlpha;
        }
        else
        {
            dir = 1f;
        }
        float time = 0f;
        while (time < duration)
        {
            alpha = Mathf.Lerp(alpha, 1 - dir + minimumAlpha, time / duration);
            mat.SetColor("_Color", new Color(mat.color.r, mat.color.g, mat.color.b, alpha));
            time += Time.deltaTime;
            yield return null;
        }
    }
}
