using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TransformationManager : MonoBehaviour
{

    public Vector3 maxSize;
    public Vector3 minSize;

    public void Scale(Vector3 scaleVector)
    {
        Vector3 newScale = new Vector3(
            transform.localScale.x * scaleVector.x,
            transform.localScale.y * scaleVector.y,
            transform.localScale.z * scaleVector.z);
        // if there are size restrictions defined => first check if they are violated
        // check restrictions
        if ((newScale.x <= maxSize.x && newScale.y <= maxSize.y && newScale.z <= maxSize.z) &&
            (newScale.x >= minSize.x && newScale.y >= minSize.y && newScale.z >= minSize.z))
        {
            transform.localScale = newScale;
        }
        else if (newScale.x < minSize.x && newScale.y < minSize.y && newScale.z < minSize.z)
        {
            transform.localScale = minSize;
        }
        else
        {
            transform.localScale = maxSize;
        }
    }

    public void Rotate(Vector3 axis, float angle)
    {
        transform.Rotate(axis, angle, Space.World);
    }

    internal void Translate(Vector3 movement)
    {
        transform.Translate(movement, Space.World);
    }
}
