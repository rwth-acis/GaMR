using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Handles the visibility of the attached gameobject
/// If the user focuses the gameobject, it becomes intransparent
/// else: it has a predefined amount of transparency
/// </summary>
public class FadeInOnFocus : MonoBehaviour, IFocusable
{
    [Tooltip("Time that the fading between the transparent state and fully visible state should take")]
    public float fadeDuration = 1f;
    [Tooltip("Alpha value of the transparent state")]
    public float minimumAlpha = 0.4f;

    private Material mat;
    private float alpha = 1f;
    private bool inFocus;
    private bool matInitializedInLastFrame = false;

    /// <summary>
    /// Initialization
    /// fetches the renderer of the gameobject
    /// </summary>
    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    /// <summary>
    /// called when the user starts focusing the gameobject
    /// starts fading in-Coroutine
    /// </summary>
    public void OnFocusEnter()
    {
        if (mat != null)
        {
            StopAllCoroutines();
            StartCoroutine(Fade(true, fadeDuration));
        }
        inFocus = true;
    }

    /// <summary>
    /// called when the user stops focusing the gameobject
    /// starts fading out-Coroutine
    /// </summary>
    public void OnFocusExit()
    {
        if (mat != null)
        {
            StopAllCoroutines();
            StartCoroutine(Fade(false, fadeDuration));
        }
        inFocus = false;
    }

    /// <summary>
    /// called every frame
    /// Correction of Initialization by waiting for the material initialization and then fading out the object
    /// </summary>
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

    /// <summary>
    /// Fading coroutine
    /// </summary>
    /// <param name="fadeIn">Direction of the fade: if true: fade from transparent state to solid state</param>
    /// <param name="duration">Time in seconds that the fading takes</param>
    /// <returns></returns>
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
            // blend the alpha value based on the ratio of elapsed time to the whole duration
            alpha = Mathf.Lerp(alpha, 1 - dir + minimumAlpha, time / duration);
            // updat the alpha value on the material
            mat.SetColor("_Color", new Color(mat.color.r, mat.color.g, mat.color.b, alpha));
            // add the time which was between the frames to the overall time
            time += Time.deltaTime;
            yield return null;
        }
    }
}
