using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geometry {

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
