using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// script which keeps the transform at the bottom of the parent transform's bounds
/// the bounds of the parent are defined by a box collider
/// Used for the bounding box to position its menu at the bottom - independent of the rotation of the bounding box
/// </summary>
public class KeepAtBottom : MonoBehaviour
{

    private BoxCollider coll;
    private ObjectInfo info;
    private float maximumExtend;
    private Bounds lastBounds;

    // get necessary components
    void Start()
    {
        coll = transform.parent.GetComponent<BoxCollider>();
        info = transform.parent.GetComponent<BoundingBoxInfo>().ObjectInfo;
    }

    /// <summary>
    /// check every frame: if the collider is enabled get its bounds and position the object at the bottom of the bounds
    /// </summary>
    void Update()
    {
        // if the collider is disabled, the bounds are incorrect
        // however, this does not matter as the object cannot be moved if the bounding box is disabled
        if (coll.enabled)
        {
            lastBounds = coll.bounds;
        }
        transform.position = lastBounds.center - new Vector3(0, lastBounds.extents.y, 0);
    }
}
