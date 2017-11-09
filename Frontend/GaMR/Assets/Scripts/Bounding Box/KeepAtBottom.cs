using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// script which keeps the transform at the bottom of the parent transform's bounds
/// the bounds of the parent are defined by a box collider
/// </summary>
public class KeepAtBottom : MonoBehaviour
{

    private BoxCollider coll;
    private ObjectInfo info;
    private float maximumExtend;
    private Bounds lastBounds;

    // Use this for initialization
    void Start()
    {
        coll = transform.parent.GetComponent<BoxCollider>();
        info = transform.parent.GetComponent<BoundingBoxInfo>().ObjectInfo;
    }

    // Update is called once per frame
    void Update()
    {
        if (coll.enabled)
        {
            lastBounds = coll.bounds;
        }
            transform.position = lastBounds.center - new Vector3(0, lastBounds.extents.y, 0);
    }
}
