using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepInFrontOfCamera : MonoBehaviour, IInputClickHandler
{

    public float distanceInFrontOfCamera = 2f;
    public bool isBeingPlaced = false;

    public virtual void OnInputClicked(InputClickedEventData eventData)
    {
        isBeingPlaced = !isBeingPlaced;
    }

    private void Update()
    {
        if (isBeingPlaced)
        {


            transform.position =
                Camera.main.transform.position + distanceInFrontOfCamera * Camera.main.transform.forward;
        }
    }
}
