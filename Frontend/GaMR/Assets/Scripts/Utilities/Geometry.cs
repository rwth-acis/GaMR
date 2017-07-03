using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// collects geometry realted helper methods
/// </summary>
public class Geometry {

    /// <summary>
    /// Calculates the bounds of an object if it was not rotated
    /// This is basically the real size of the object
    /// Just calling the bounds returns the bounds of the rotated object
    /// </summary>
    /// <param name="transform">The transform to measure</param>
    /// <returns>The bounds of the unrotated object</returns>
	public static Bounds GetBoundsIndependentFromRotation(Transform transform)
    {
        Quaternion rotation = transform.rotation;
        transform.rotation = Quaternion.identity;

        Renderer rend = transform.GetComponent<Renderer>();
        Bounds res = rend.bounds;

        transform.rotation = rotation;
        return res;
    }
}
