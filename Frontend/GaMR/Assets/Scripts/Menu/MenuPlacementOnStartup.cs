using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPlacementOnStartup : MonoBehaviour
{
    public Vector3 realForward = new Vector3(0, 0, -1);

    void Start()
    {
        Vector3 targetPos = Camera.main.transform.position + Camera.main.transform.forward * 2f;
        transform.position = targetPos;
        FaceUser(-2f * Camera.main.transform.forward);
    }

    private Transform GetGlobalParent()
    {
        Transform current = transform;
        while (current.parent != null)
        {
            current = current.parent;
        }

        return current;
    }

    private void FaceUser(Vector3 vectorToCam)
    {
        transform.rotation = Quaternion.LookRotation(vectorToCam, Vector3.up);
        transform.rotation = Quaternion.LookRotation(transform.TransformDirection(realForward));
    }
}
