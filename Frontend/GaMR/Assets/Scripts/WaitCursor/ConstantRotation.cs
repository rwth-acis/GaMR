using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// applies a constant rotation around the z axis if it is attached to a gameobject
/// </summary>
public class ConstantRotation : MonoBehaviour {

    [Tooltip("The rotation speed in degrees per second")]
    public float degreesPerSecond = 50;
	


	/// <summary>
    /// Rotates the object with repsect to the time which has passed since the last update call
    /// </summary>
	void Update () {
        transform.Rotate(0, 0, degreesPerSecond * Time.deltaTime);
	}
}
