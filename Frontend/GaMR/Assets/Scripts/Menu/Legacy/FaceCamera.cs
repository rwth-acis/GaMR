using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// objects with this component always face the player
/// </summary>
public class FaceCamera : MonoBehaviour
{

    public Vector3 rotationOffset = Vector3.zero;

    /// <summary>
    /// gets the vector between the transform's position and the camera's position
    /// rotates the object according to this vector
    /// </summary>
    public void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        transform.Rotate(rotationOffset);
    }
}
