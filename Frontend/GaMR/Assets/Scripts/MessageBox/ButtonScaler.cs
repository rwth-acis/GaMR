using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// script which handles the scaling of the button
/// the adaptive scaling only works in the editor in order to save performance
/// </summary>
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

    [Tooltip("The size of the button's icon")]
    public float iconSize = 0.05f;
    [Tooltip("The size of the button's caption")]
    public float captionSize = 0.006f;
    [Tooltip("The width of the border around the button")]
    public float borderWidth = 1f;
    private float frameSize = 0.01423f; // this is an internal value which scales the frame to the correct size
    [Tooltip("If this is enabled, the icon and text will also be scaled if the button is resized")]
    public bool scaleComponentsWithButton = true;

    /// <summary>
    /// Initialization function which is executed with the first update
    /// In Awake and Start many objects do not seem to be available yet
    /// </summary>
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

    /// <summary>
    /// Undoes any scaling inheritage of the transform's child object
    /// This will give the child a constant size independent of the parent scale
    /// </summary>
    /// <param name="child">The child which should be rescaled. This should be a child of the
    /// transform to which this script is attached.</param>
    /// <param name="scaleFactor">Determines the constant uniform size of the child object</param>
    private void UndoScaling(Transform child, float scaleFactor)
    {
        UndoScaling(child, new Vector3(scaleFactor, scaleFactor, scaleFactor));
    }

    /// <summary>
    /// Undoes any scaling inheritage of the transform's child object
    /// This will give the child a constant size independent of the parent scale
    /// </summary>
    /// <param name="child">The child which should be rescaled. This should be a child of the
    /// transform to which this script is attached.</param>
    /// <param name="scaleFactors">Determines the constant size of the child object. The size can be specified axis-independent</param>
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

    /// <summary>
    /// Function which uniformly scales the given child, but only if the transform's ratio allows it
    /// The child is scaled by the minimum factor of the parent scaling.
    /// e.g. if the transform is scaled by (5,1), the child is scaled by (1,1) 
    /// and if the transform is scaled by (2,3) the child is scaled by (2,2)
    /// </summary>
    /// <param name="child">The child to scale</param>
    private void ScaleProportionally(Transform child)
    {
        float scaleFactor = Mathf.Min(ratio.y, ratio.z);
        child.localScale *= scaleFactor;
    }

    /// <summary>
    /// This scales the 9-patch sprite of the frame so that it fits the button.
    /// Furthermore there is functionality included which also scales the sprite
    /// itself and compensates this in the 9-patch image. This scaling-combination alters the width of the frame
    /// </summary>
    private void ScaleFrame()
    {
        frame.size = new Vector2(
            originalFrameSize.x * ratio.z,
            originalFrameSize.y * ratio.y
            );
        frame.size /= borderWidth;
        frame.transform.localScale *= borderWidth;
    }

    /// <summary>
    /// this script is executed in the editor where Update is called everytime that something in the scene has changed
    /// it handles the scaling of the individual button components
    /// </summary>
    void Update()
    {
        // initialize if update is called for the first time
        if (firstUpdate)
        {
            Initialize();
            firstUpdate = false;
        }

        // avoid border width of 0 => a border width of 0 causes division by 0
        if (borderWidth == 0)
        {
            borderWidth = 1;
        }

        // undo the parent scaling on the button components
        UndoScaling(icon, iconSize);
        UndoScaling(frameTransform, frameSize);
        UndoScaling(captionTransform, captionSize);

        // make sure that no scale is 0 or else the button will not be visible anymore
        if (transform.localScale.x != 0 && transform.localScale.y != 0 && transform.localScale.z != 0)
        {
            ratio = new Vector3(
                transform.localScale.x / originalSize.x,
                transform.localScale.y / originalSize.y,
                transform.localScale.z / originalSize.z
                );
        }

        // handle the scaling of the components
        if (scaleComponentsWithButton)
        {
            ScaleProportionally(icon);
            ScaleProportionally(captionTransform);
        }

        // scale the frame to fit the button
        ScaleFrame();
    }
#endif
}
