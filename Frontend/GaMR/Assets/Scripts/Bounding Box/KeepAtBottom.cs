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

    // Use this for initialization
    void Start()
    {
        coll = transform.parent.GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = coll.bounds.center - new Vector3(0, coll.bounds.extents.y, 0);
    }
}
