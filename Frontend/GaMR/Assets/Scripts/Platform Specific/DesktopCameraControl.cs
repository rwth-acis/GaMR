using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesktopCameraControl : Singleton<DesktopCameraControl> {

    public float movementSpeed = 0.1f;
    public bool InputEnabled { get; set; }

    private void Start()
    {
        InputEnabled = true;
    }

    private void FixedUpdate()
    {
        if (InputEnabled)
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
}
