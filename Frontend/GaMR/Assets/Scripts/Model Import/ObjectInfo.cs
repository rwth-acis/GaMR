using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component which stores all information which are specific to one 3D model
/// </summary>
public class ObjectInfo : MonoBehaviour
{

    /// <summary>
    /// The name of the 3D model
    /// </summary>
    public string ModelName
    {
        get; set;
    }

    public Vector3 Size
    {
        get
        {
            return new Vector3(Bounds.size.x * transform.localScale.x,
          Bounds.size.y * transform.localScale.y * Bounds.size.z * transform.localScale.z);
        }
    }

    public Bounds Bounds
    {
        get; set;
    }
}
