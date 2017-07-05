using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Collection of actions which are performed on the bounding box and by the attached menu
/// </summary>
public class BoundingBoxActions : MonoBehaviour
{

    private bool boundingBoxVisible = true;
    public List<Transform> boundingBoxPieces;
    private BoxCollider coll;
    private AnnotationManager annotationManager;

    /// <summary>
    /// Get the necessary components: the collider of the bounding box and its annotationManager
    /// </summary>
    public void Start()
    {
        coll = GetComponent<BoxCollider>();
        annotationManager = gameObject.GetComponentInChildren<AnnotationManager>();
    }

    /// <summary>
    /// toggles the visibility of the bounding boxx and whether or not its collider should be active
    /// </summary>
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

    /// <summary>
    /// toggles whether or not it is possible to add new annotations to the model by tapping on it
    /// </summary>
    public void ToogleEditMode()
    {
        annotationManager.EditMode = !annotationManager.EditMode;
    }

    /// <summary>
    /// Shows or hides all control handles which are attached to the bounding box
    /// </summary>
    /// <param name="active">The target visibility</param>
    private void ToggleControls(bool active)
    {
        coll.enabled = active;
        foreach(Transform boxPart in boundingBoxPieces)
        {
            boxPart.gameObject.SetActive(active);
        }
    }

    /// <summary>
    /// Destroys the bounding box and its content
    /// </summary>
    public void DeleteObject()
    {
        Destroy(gameObject);
    }
}
