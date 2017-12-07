using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The script positions a component relative to another
/// It is possible to respect the proportional scaling of the parent
/// </summary>
[ExecuteInEditMode]
public class ProportionalConstantAlignment : MonoBehaviour
{
    [Tooltip("The transform to which this transform be aligned")]
    public Transform alignTo;
    [Tooltip("The offset to the chosen alignment anchor")]
    public Vector3 constantOffset;

    private bool firstUpdate = true;

    [Tooltip("If this is enabled, the positioning will scale with the proportional scale of the parent")]
    public bool scaleProportionalToParent = true;

    private Vector3 originalParentSize;
    private Vector3 ratio = Vector3.one;

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

        if (alignTo != null)
        {
            if (scaleProportionalToParent)
            {
                // consider proportional scaling factor of the parent
                float scaleFactor = Mathf.Min(ratio.y, ratio.z);
                transform.position = alignTo.position + scaleFactor * constantOffset;
            }
            else
            {
                // place object according to alignment anchor and offset
                transform.position = alignTo.position + constantOffset;
            }
        }
    }
}
