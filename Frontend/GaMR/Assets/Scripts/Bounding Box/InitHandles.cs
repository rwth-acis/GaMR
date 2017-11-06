using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Placed on the parent-object of all scale- and rotation-handles
/// distributes the DragHandle script to them and sets it up
/// NOTE: since this script relys on the fact that the bounding-box has not yet been reshaped this script needs to be called before any other
/// </summary>
public class InitHandles : MonoBehaviour
{

    Transform toManipulate;
    List<Transform> scaleHandles;
    List<Transform> rotationHandles;
    public float scaleSpeed;
    public float rotationSpeed;

    // Use this for initialization
    void Start()
    {

        // this should point to the bounding-box object
        toManipulate = transform.parent;

        scaleHandles = new List<Transform>();
        rotationHandles = new List<Transform>();

        // get the parent which holds the scale handles
        Transform scaleParent = transform.Find("Scale");
        // get the parent which holds the rotation handles
        Transform rotationParent = transform.Find("Rotation");
        // add each handle to the corresponding list
        foreach (Transform child in scaleParent)
        {
            scaleHandles.Add(child);
        }

        foreach (Transform child in rotationParent)
        {
            rotationHandles.Add(child);
        }

        // distribute the drag script to all handles
        foreach (Transform handle in scaleHandles)
        {
            DragHandle handleScript = handle.gameObject.AddComponent<DragHandle>();
            // initialize the script: add the target, the gesture orientation and the handle type
            handleScript.toManipulate = toManipulate;
            handleScript.handleType = HandleType.SCALE;
            handleScript.speed = scaleSpeed;
        }

        foreach(Transform handle in rotationHandles)
        {
            DragHandle handleScript = handle.gameObject.AddComponent<DragHandle>();
            // initialize the script: add the target, the gesture orientation and the handle type
            handleScript.toManipulate = toManipulate;
            handleScript.handleType = HandleType.ROTATE;
            // define the axis
            // determine rotation axis from the handle's position on the unit cube
            handleScript.gestureOrientation = DetermineRotationAxis(handle.transform.localPosition);
            handleScript.speed = rotationSpeed;
        }
    }

    /// <summary>
    /// determines the rotation axis of the handle based on its position on the unit cube
    /// e.g. a handle at the position (0.5, 0, 0.5) will rotate around the Y-axis
    /// </summary>
    /// <param name="handlePosition">The relative position on the unit cube</param>
    /// <returns></returns>
    Vector3 DetermineRotationAxis(Vector3 handlePosition)
    {
        if (handlePosition.x == 0)
        {
            return Vector3.right;
        }
        else if (handlePosition.y == 0)
        {
            return Vector3.down;
        }
        else if (handlePosition.z == 0)
        {
            return Vector3.forward;
        }
        else
        {
            throw new System.Exception("Handle seems to be placed at the wrong position. Could not determine rotation axis");
        }
    }
}
