using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingBoxActions : MonoBehaviour
{

    private bool boundingBoxVisible = true;
    public List<Transform> boundingBoxPieces;
    private BoxCollider coll;
    private AnnotationManager annotationManager;

    // Use this for initialization
    public void Start()
    {
        coll = GetComponent<BoxCollider>();
        annotationManager = gameObject.GetComponentInChildren<AnnotationManager>();
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

    public void ToogleEditMode()
    {
        annotationManager.EditMode = !annotationManager.EditMode;
    }

    private void ToggleControls(bool active)
    {
        coll.enabled = active;
        foreach(Transform boxPart in boundingBoxPieces)
        {
            boxPart.gameObject.SetActive(active);
        }
    }

    public void DeleteObject()
    {
        Destroy(gameObject);
    }
}
