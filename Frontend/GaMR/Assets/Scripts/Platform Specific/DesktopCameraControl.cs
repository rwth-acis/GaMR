using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesktopCameraControl : MonoBehaviour {

    public float movementSpeed = 0.1f;

    private void FixedUpdate()
    {
        float horizontalAmount = Input.GetAxis("Horizontal");
        float verticalAmount = Input.GetAxis("Vertical");
        float upDownAmount = Input.GetAxis("UpDown");

        transform.position += transform.rotation * (movementSpeed * new Vector3(horizontalAmount, upDownAmount, verticalAmount));

        if (Input.GetMouseButton(1))
        {
            float rotationX = Input.GetAxis("Mouse X");
            float rotationY = Input.GetAxis("Mouse Y");

            transform.localEulerAngles = transform.rotation.eulerAngles + new Vector3(-rotationY, rotationX, 0);
        }
    }
}
