using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InFrontOfCamera : Singleton<InFrontOfCamera>
{
    public Vector3 Position
    {
        get { return transform.position; }
    }

    private void Update()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        int layerMask = (1 << 10);
        layerMask |= (1 << 2);
        layerMask = ~layerMask;
        RaycastHit hitInfo;
        Vector3 vectorFromCamera = Camera.main.transform.forward;
        if (Physics.Raycast(ray, out hitInfo, 2f, layerMask))
        {
            vectorFromCamera *= hitInfo.distance;
        }
        else
        {
            vectorFromCamera *= 2f;
        }

        transform.position = Camera.main.transform.position + vectorFromCamera;
    }
}
