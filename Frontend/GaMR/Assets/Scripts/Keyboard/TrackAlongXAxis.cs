using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Makes an object follow the the user's gaze on a fixed height
/// </summary>
public class TrackAlongXAxis : MonoBehaviour {

    public float distance = 3f;
    private Transform parent;

    /// <summary>
    /// the velocity at which the transform is currently moving to its target position
    /// </summary>
    private Vector3 velocity = Vector3.zero;
    /// <summary>
    /// the time it takes to reach the target position
    /// </summary>
    public float smoothTime = 0.3f;

    /// <summary>
    /// Gets the necessary components: the gameobject's parent and the gazeManager
    /// </summary>
    public void Start()
    {
        parent = transform.parent;
    }

    public void Update()
    {
        // convert the positon of the cursor to the parent's local space
        Vector3 cursorRelativeToKeyboard = parent.InverseTransformPoint(GazeManager.Instance.HitPosition);

        // determine where the object should be
        Vector3 targetPos = new Vector3(
            cursorRelativeToKeyboard.x,
            transform.localPosition.y,
            transform.localPosition.z);

        // dampen the movement for more natural behavior
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPos, ref velocity, smoothTime);
    }
}
