using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// registers taps which are made in the air without a target or on a wall
/// </summary>
public class EmptyTapManager : MonoBehaviour, IInputHandler
{

    private Transform spatialMappingObject;

    /// <summary>
    /// adds a global listener to the inputmanager so that the component is informed about all taps
    /// fetches necessary components
    /// </summary>
    private void Start()
    {
        InputManager.Instance.AddGlobalListener(gameObject);
        spatialMappingObject = GameObject.Find("SpatialMappingManager").transform;
    }

    /// <summary>
    /// An empty tap was detected: open or close the main menu
    /// </summary>
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

    /// <summary>
    /// Called when the user taps anywhere
    /// checks if the tap had no target or targeted a wall
    /// </summary>
    /// <param name="eventData"></param>
    public void OnInputDown(InputEventData eventData)
    {
        if (GazeManager.Instance.HitObject == null || GazeManager.Instance.HitObject.transform.parent == spatialMappingObject)
        {
            EmptyTapDetected();
        }
    }
}
