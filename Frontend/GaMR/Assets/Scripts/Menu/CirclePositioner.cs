using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// makes an object float on a circle
/// the object is always on the point of the circle which is closest to the camera
/// </summary>
public class CirclePositioner : MonoBehaviour {

    [Tooltip("The center of the circle")]
    public Transform center;
    [Tooltip("The size of the object which will have the circle around it")]
    public Transform parentSize;
    private float distance;

    // the velocity at which the menu is currently moving to its target position
    private Vector3 velocity = Vector3.zero;
    // the time it takes to reach the target position
    public float smoothTime = 0.3f;
	
	/// <summary>
    /// keeps the attached GameObject at the position on the circle which is closest to the user
    /// </summary>
	void Update () {
        // calculate the maximum distance to encapsualte the object
        distance = 1.2f * new Vector2(parentSize.localScale.x/2, parentSize.localScale.z/2).magnitude;
        // get the vector from the center to the camera 
        // and add it (scaled by distance) to the position of the center to get the point on the circle
        Vector3 centerToCamera = Camera.main.transform.position - center.position;
        Vector3 pos = center.position + distance * new Vector3(
            centerToCamera.normalized.x,
            0,
            centerToCamera.normalized.z);
        //transform.position = pos; // directly jumping to the position
        // dampen the movement a bit so that is smoothly transitions to the position
        // instead of always being there
        transform.position = Vector3.SmoothDamp(transform.position, pos, ref velocity, smoothTime);

	}
}
