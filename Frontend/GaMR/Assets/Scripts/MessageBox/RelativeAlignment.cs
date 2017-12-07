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


    private Transform previousAlignTo;
    private bool firstUpdate = true;
    private Vector3 originalParentSize;
    private Vector3 ratio = Vector3.one;

    /// <summary>
    /// destroys the script if not in editor to save performance on the final product
    /// </summary>
    private void Awake()
    {
        
    }


    /// <summary>
    /// Initalization of the required variables in the first update call
    /// </summary>
    private void Initialize()
    {
        originalParentSize = transform.parent.localScale;
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

        // make sure that the scale of the parent is not 0
        if (transform.localScale.x != 0 && transform.localScale.y != 0 && transform.localScale.z != 0)
        {
            ratio = new Vector3(
                transform.parent.localScale.x / originalParentSize.x,
                transform.parent.localScale.y / originalParentSize.y,
                transform.parent.localScale.z / originalParentSize.z
                );
        }

        if (alignmentMode == AlignmentMode.TRANSFORM)
        {
            // restore the transform if it was set in the align-to-transform mode
            if (alignTo == null && previousAlignTo != null)
            {
                alignTo = previousAlignTo;
            }

            if (alignTo != null)
            {
                // automatically calculate the offset to the new transform based on the current position
                // this streamlines the positioning process as the current position is kept
                if (previousAlignTo != alignTo)
                {
                    constantOffset = transform.position - alignTo.position;
                }

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

            switch (alignmentMode)
            {
                case AlignmentMode.LEFT_PARENT:
                    alignmentPos = transform.parent.TransformPoint(new Vector3(0, 0, 0.5f));
                    AlignToPosition(alignmentPos);
                    break;
                case AlignmentMode.RIGHT_PARENT:
                    alignmentPos = transform.parent.TransformPoint(new Vector3(0, 0, -0.5f));
                    AlignToPosition(alignmentPos);
                    break;
                case AlignmentMode.TOP_PARENT:
                    alignmentPos = transform.parent.TransformPoint(new Vector3(0, 0.5f, 0));
                    AlignToPosition(alignmentPos);
                    break;
                case AlignmentMode.BOTTOM_PARENT:
                    alignmentPos = transform.parent.TransformPoint(new Vector3(0, -0.5f, 0));
                    AlignToPosition(alignmentPos);
                    break;
            }
        }
    }

    private void AlignToPosition(Vector3 position)
    {
        if (scaleProportionalToParent)
        {
            // consider proportional scaling factor of the parent
            float scaleFactor = Mathf.Min(ratio.y, ratio.z);
            transform.position = position + scaleFactor * constantOffset;
        }
        else
        {
            // place object according to alignment anchor and offset
            transform.position = position + constantOffset;
        }
    }

    public enum AlignmentMode
    {
        TRANSFORM, LEFT_PARENT, RIGHT_PARENT, TOP_PARENT, BOTTOM_PARENT
    }
}
