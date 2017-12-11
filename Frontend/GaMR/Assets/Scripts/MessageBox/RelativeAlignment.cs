using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The script positions a component relative to another
/// It is possible to respect the proportional scaling of the parent
/// the script is only intended for design help in the editor and is automatically destroyed in a build
/// </summary>
[ExecuteInEditMode]
public class RelativeAlignment : MonoBehaviour
{
    public AlignmentMode alignmentMode;

    [Tooltip("The transform to which this transform be aligned; can only be set in TRANSFORM-mode")]
    public Transform alignTo;

    [Tooltip("The offset to the chosen alignment anchor")]
    public Vector3 constantOffset;

    [Tooltip("If this is enabled, the positioning will scale with the proportional scale of the parent")]
    public bool scaleProportionalToParent = true;
    [Tooltip("Set a direction to 0 to disable proportional scaling into this direction")]
    public Vector3 scaleEffect = Vector3.one;


    private Transform previousAlignTo;
    private AlignmentMode previousAlignmentMode;
    private bool firstUpdate = true;
    private Vector3 ratio = Vector3.one;

    /// <summary>
    /// destroys the script if not in editor to save performance on the final product
    /// </summary>
    private void Awake()
    {
#if !UNITY_EDITOR
        Destroy(this);
#endif
    }


    /// <summary>
    /// Initalization of the required variables in the first update call
    /// </summary>
    private void Initialize()
    {
    }

    /// <summary>
    /// This script is executed in the editor and so this will be called if something changes in the editor
    /// It keeps track of the parents changes in scale and calculates the correct alignment
    /// </summary>
    void Update()
    {
        if (firstUpdate)
        {
            Initialize();
            firstUpdate = false;
        }

        // clean up the scale effect to be either 1 or 0
        scaleEffect = new Vector3(
            Mathf.Max(0, Mathf.Min(1, Mathf.Round(scaleEffect.x))),
            Mathf.Max(0, Mathf.Min(1, Mathf.Round(scaleEffect.y))),
            Mathf.Max(0, Mathf.Min(1, Mathf.Round(scaleEffect.z))));

        // make sure that the scale of the parent is not 0
        if (transform.localScale.x != 0 && transform.localScale.y != 0 && transform.localScale.z != 0)
        {
            ratio = transform.parent.localScale;
            Transform current = transform.parent.parent;
            while(current != null)
            {
                ratio.Scale(current.localScale);
                current = current.parent;
            }
        }


        if (alignmentMode == AlignmentMode.NONE)
        {
            alignTo = null;
            previousAlignmentMode = alignmentMode;
            return;
        }


        if (alignmentMode == AlignmentMode.TRANSFORM)
        {
            // if we just got into this mode => restore alignment transform
            if (previousAlignmentMode != alignmentMode)
            {
                // restore the transform if it was set in the align-to-transform mode
                if (alignTo == null && previousAlignTo != null)
                {
                    alignTo = previousAlignTo;
                    AutoCalculateOffset(alignTo.position);
                }
            }

            // if the target transform changed:
            // calculate the offset so that the object itself stays at the same position
            if (alignTo != null && alignTo != previousAlignTo)
            {
                AutoCalculateOffset(alignTo.position);
            }

            if (alignTo != null)
            {
                AlignToPosition(alignTo.position);
            }

            // store the alignTo in order to detect changes
            previousAlignTo = alignTo;
        }
        else
        {
            // delete the reference of the alignTo to indicate that this field not used in another mode
            alignTo = null;


            Vector3 alignmentPos = new Vector3();

            // get alignment position
            switch (alignmentMode)
            {
                case AlignmentMode.CENTER:
                    alignmentPos = transform.parent.TransformPoint(Vector3.zero);
                    break;
                case AlignmentMode.LEFT:
                    alignmentPos = transform.parent.TransformPoint(new Vector3(0, 0, 0.5f));
                    break;
                case AlignmentMode.RIGHT:
                    alignmentPos = transform.parent.TransformPoint(new Vector3(0, 0, -0.5f));
                    break;
                case AlignmentMode.TOP:
                    alignmentPos = transform.parent.TransformPoint(new Vector3(0, 0.5f, 0));
                    break;
                case AlignmentMode.BOTTOM:
                    alignmentPos = transform.parent.TransformPoint(new Vector3(0, -0.5f, 0));
                    break;
                case AlignmentMode.TOP_LEFT:
                    alignmentPos = transform.parent.TransformPoint(new Vector3(0, 0.5f, 0.5f));
                    break;
                case AlignmentMode.TOP_RIGHT:
                    alignmentPos = transform.parent.TransformPoint(new Vector3(0, 0.5f, -0.5f));
                    break;
                case AlignmentMode.BOTTOM_LEFT:
                    alignmentPos = transform.parent.TransformPoint(new Vector3(0, -0.5f, 0.5f));
                    break;
                case AlignmentMode.BOTTOM_RIGHT:
                    alignmentPos = transform.parent.TransformPoint(new Vector3(0, -0.5f, -0.5f));
                    break;
                default:
                    alignmentPos = transform.position;
                    break;
            }

            // automatically change the offset if the alignment mode is changed to fit to the calculated alignment
            if (previousAlignmentMode != alignmentMode)
            {
                AutoCalculateOffset(alignmentPos);
            }

            // align to the calculated position
            AlignToPosition(alignmentPos);
        }

        previousAlignmentMode = alignmentMode;
    }

    private void AlignToPosition(Vector3 position)
    {
        if (scaleProportionalToParent)
        {
            // consider proportional scaling factor of the parent
            float scaleFactor = Mathf.Min(ratio.y, ratio.z);
            // only take the scale factor if the direction is "enabled"
            transform.position = position + transform.parent.rotation * Vector3.Scale(
                new Vector3(
                    scaleEffect.x == 0 ? 1 : scaleFactor,
                    scaleEffect.y == 0 ? 1 : scaleFactor,
                    scaleEffect.z == 0 ? 1 : scaleFactor
                    ),
                constantOffset);
        }
        else
        {
            // place object according to alignment anchor and offset
            transform.position = position + transform.parent.rotation * constantOffset;
        }
    }

    private void AutoCalculateOffset(Vector3 targetPosition)
    {
        // automatically calculate the offset to the new transform based on the current position
        // this streamlines the positioning process as the current position is kept
        constantOffset = transform.position - targetPosition;

        constantOffset = Quaternion.Inverse(transform.parent.rotation) * constantOffset;

        if (scaleProportionalToParent)
        {
            // consider proportional scaling factor of the parent
            float scaleFactor = Mathf.Min(ratio.y, ratio.z);
            // undo the scaling factor since it will be multiplied with the offset in the positioning
            constantOffset = Vector3.Scale(constantOffset,
                new Vector3(
                    scaleEffect.x == 0 ? 1 : 1 / scaleFactor,
                    scaleEffect.y == 0 ? 1 : 1 / scaleFactor,
                    scaleEffect.z == 0 ? 1 : 1 / scaleFactor)
                );
        }
    }

    public enum AlignmentMode
    {
        NONE, TRANSFORM, CENTER, LEFT, RIGHT, TOP, BOTTOM, TOP_LEFT, TOP_RIGHT, BOTTOM_LEFT, BOTTOM_RIGHT
    }
}
