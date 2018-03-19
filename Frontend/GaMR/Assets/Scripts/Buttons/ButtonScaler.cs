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
    private Transform frameTransform;
    private SpriteRenderer frame;
    private Transform icon;
    private SpriteRenderer iconRenderer;
    private Transform captionTransform;
    private Transform contentTransform;
    private Transform led;
    private bool firstUpdate = true;
    private Vector3 ratio = Vector3.one;
    private Vector2 originalFrameSize;

    [Tooltip("The size of the button's icon")]
    public float iconSize = 0.05f;
    public bool flipIconX = false;
    public bool flipIconY = false;
    [Tooltip("The size of the button's caption")]
    public float captionSize = 0.006f;
    [Tooltip("The size of the LED (if it exists)")]
    public float ledSize = 1f;
    [Tooltip("The size of the content label (if it exists)")]
    public float contentSize = 0.02f;
    [Tooltip("The width of the border around the button")]
    public float borderWidth = 1f;
    [Tooltip("If this is enabled, the icon, text and LED will also be scaled if the button is resized")]
    public bool scaleComponentsWithButton = true;

    private bool previousScaleComponentWithButton;

    private float frameSize = 0.1423f; // this is an internal value which scales the frame to the correct size

    /// <summary>
    /// makes sure that the script is only affecting the editor
    /// the script is destroyed in play mode to save performance
    /// </summary>
    private void Awake()
    {
#if !UNITY_EDITOR
        RemoveScript();
#endif

        if (Application.isEditor && Application.isPlaying)
        {
            RemoveScript();
        }
    }

    private void RemoveScript()
    {
        Destroy(this);
    }

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
        if (icon != null)
        {
            iconRenderer = icon.GetComponent<SpriteRenderer>();
        }
        captionTransform = transform.Find("Caption");

        led = transform.Find("LED");

        contentTransform = transform.Find("Content");

        // set original values:
        // these values are obtained when the button has the size (1,1)
        originalFrameSize = new Vector2(7.05f, 7.05f);
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
        if (child != null) // only do something if the child really exists
        {
            if (ratio.x != 0 && ratio.y != 0 && ratio.z != 0)
            {
                Vector3 newScale = new Vector3(
                    scaleFactors.x / ratio.x,
                    scaleFactors.y / ratio.y,
                    scaleFactors.z / ratio.z
                    );

                newScale = child.localRotation * newScale;

                newScale = new Vector3(Mathf.Abs(newScale.x), Mathf.Abs(newScale.y), Mathf.Abs(newScale.z));
                child.localScale = newScale;
            }
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
        if (child != null) // only do something if the child really exits
        {
            float scaleFactor = Mathf.Min(ratio.y, ratio.z);
            child.localScale *= scaleFactor;
        }
    }

    /// <summary>
    /// This scales the 9-patch sprite of the frame so that it fits the button.
    /// Furthermore there is functionality included which also scales the sprite
    /// itself and compensates this in the 9-patch image. This scaling-combination alters the width of the frame
    /// </summary>
    private void ScaleFrame()
    {
        if (frame != null)
        {
            frame.size = new Vector2(
                originalFrameSize.x * ratio.z,
                originalFrameSize.y * ratio.y
                );
            frame.size /= borderWidth;
            frame.transform.localScale *= borderWidth;
        }
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

        // make sure that no scale is 0 or else the button will not be visible anymore
        if (transform.localScale.x != 0 && transform.localScale.y != 0 && transform.localScale.z != 0)
        {
            ratio = transform.localScale;
            Transform current = transform.parent;
            Transform last = transform;
            while (current != null)
            {
                ratio.Scale(last.localRotation * current.localScale);
                last = current;
                current = current.parent;
            }
        }

        // undo the parent scaling on the button components
        UndoScaling(icon, iconSize);
        UndoScaling(frameTransform, frameSize);
        UndoScaling(captionTransform, captionSize);
        UndoScaling(led, ledSize);
        UndoScaling(contentTransform, contentSize);

        // handle the scaling of the components
        if (scaleComponentsWithButton)
        {
            ScaleProportionally(icon);
            ScaleProportionally(captionTransform);
            ScaleProportionally(led);
            ScaleProportionally(contentTransform);
        }

        if (iconRenderer != null)
        {
            iconRenderer.flipX = flipIconX;
            iconRenderer.flipY = flipIconY;
        }

        // scale the frame to fit the button
        ScaleFrame();

        // also fix the frame to always be in the middle of the button
        frameTransform.localPosition = new Vector3(frameTransform.localPosition.x, 0, 0);
    }
}
