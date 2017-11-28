using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ButtonScaler : MonoBehaviour
{
#if UNITY_EDITOR
    private Transform frameTransform;
    private SpriteRenderer frame;
    private Transform icon;
    private Transform captionTransform;
    private bool firstUpdate = true;
    private Vector3 originalSize;
    private Vector3 ratio = Vector3.one;
    private Vector2 originalFrameSize;

    public float iconSize = 0.05f;
    public float captionSize = 0.006f;
    public float borderWidth = 1f;
    private float frameSize = 0.01423f; // this is an internal value which scales the frame to the correct size

    public bool scaleWithButton = true;

    void Initialize()
    {
        frameTransform = transform.Find("Frame");
        if (frameTransform != null)
        {
            frame = frameTransform.GetComponent<SpriteRenderer>();
        }

        icon = transform.Find("Icon");
        captionTransform = transform.Find("Caption");

        originalSize = transform.localScale;
        originalFrameSize = frame.size;
    }

    private void UndoScaling(Transform child, float scaleFactor)
    {
        UndoScaling(child, new Vector3(scaleFactor, scaleFactor, scaleFactor));
    }

    private void UndoScaling(Transform child, Vector3 scaleFactors)
    {
        if (transform.localScale.x != 0 && transform.localScale.y != 0 && transform.localScale.z != 0)
        {
            Vector3 newScale = new Vector3(
                scaleFactors.x / transform.localScale.x,
                scaleFactors.y / transform.localScale.y,
                scaleFactors.z / transform.localScale.z
                );

            newScale = child.rotation * newScale;
            child.localScale = newScale;
        }
    }

    private void ScaleProportionally(Transform child)
    {
            float scaleFactor = Mathf.Min(ratio.y, ratio.z);
            child.localScale *= scaleFactor;
    }

    private void ScaleFrame()
    {
        frame.size = new Vector2(
            originalFrameSize.x * ratio.z,
            originalFrameSize.y * ratio.y
            );
        frame.size /= borderWidth;
        frame.transform.localScale *= borderWidth;
    }

    void Update()
    {
        if (firstUpdate)
        {
            Initialize();
            firstUpdate = false;
        }

        if (borderWidth == 0)
        {
            borderWidth = 1;
        }

        UndoScaling(icon, iconSize);
        UndoScaling(frameTransform, frameSize);
        UndoScaling(captionTransform, captionSize);


        if (transform.localScale.x != 0 && transform.localScale.y != 0 && transform.localScale.z != 0)
        {
            ratio = new Vector3(
                transform.localScale.x / originalSize.x,
                transform.localScale.y / originalSize.y,
                transform.localScale.z / originalSize.z
                );
        }

        if (scaleWithButton)
        {
            ScaleProportionally(icon);
            ScaleProportionally(captionTransform);
        }
        ScaleFrame();
    }
#endif
}
