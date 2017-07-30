using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EmptyTapManager : MonoBehaviour, IInputHandler
{

    Transform spatialMappingObject;

    void Start()
    {
        InputManager.Instance.AddGlobalListener(gameObject);
        spatialMappingObject = GameObject.Find("SpatialMappingManager").transform;
    }

    private void EmptyTapDetected()
    {
        if (MainMenu.Instance == null || MainMenu.Instance.gameObject.activeSelf == false)
        {
            MainMenu.Show();
        }
        else if (MainMenu.Instance.gameObject.activeSelf == true)
        {
            MainMenu.Close();
        }
    }

    public void OnInputUp(InputEventData eventData)
    {
    }

    public void OnInputDown(InputEventData eventData)
    {
        if (GazeManager.Instance.HitObject == null || GazeManager.Instance.HitObject.transform.parent == spatialMappingObject)
        {
            EmptyTapDetected();
        }
    }
}
