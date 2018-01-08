using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToCameraOnYAxis : MonoBehaviour
{

    public float rotationOffset = 0f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Quaternion targetRot = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        transform.rotation = Quaternion.Euler(new Vector3(
            transform.rotation.eulerAngles.x,
            targetRot.eulerAngles.y + rotationOffset,
            transform.rotation.eulerAngles.z));
    }
}
