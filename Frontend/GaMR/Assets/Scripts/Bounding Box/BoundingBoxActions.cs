using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingBoxActions : MonoBehaviour
{

    private bool boundingBoxVisible = true;
    public List<Transform> boundingBoxPieces;
    private BoxCollider coll;

    // Use this for initialization
    public void Start()
    {
        coll = GetComponent<BoxCollider>();
    }

    public void ToggleBoundingBox()
    {
        if (boundingBoxVisible)
        {
            ToggleControls(false);
            boundingBoxVisible = false;
        }
        else
        {
            ToggleControls(true);
            boundingBoxVisible = true;
        }
    }

    private void ToggleControls(bool active)
    {
        coll.enabled = active;
        foreach(Transform boxPart in boundingBoxPieces)
        {
            boxPart.gameObject.SetActive(active);
        }
    }
}
