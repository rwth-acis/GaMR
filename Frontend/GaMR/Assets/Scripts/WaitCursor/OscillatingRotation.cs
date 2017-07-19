using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Applies a rotation to the object's z axis. The speed of the rotation will oscillate periodically.
/// </summary>
public class OscillatingRotation : MonoBehaviour {

    [Tooltip("The rotation speed in degres per second")]
    public float degreesPerSecond = 50;

    /// <summary>
    /// Applies the rotation to the object with respect to the time that has passed since the last update call
    /// </summary>
    void Update()
    {
        transform.Rotate(0, 0, Mathf.Sin(Time.time) * degreesPerSecond * Time.deltaTime);
    }
}
