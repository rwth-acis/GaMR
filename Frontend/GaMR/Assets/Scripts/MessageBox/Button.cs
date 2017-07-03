using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Registers InputEvents and redirects them to a specified method
/// </summary>
public class Button : MonoBehaviour, IInputHandler {

    /// <summary>
    /// The method which should be called if the button is pressed
    /// </summary>
    public Action OnPressed;

    /// <summary>
    /// This is called if the user starts a tap gesture on the gameobject to which this component is attached
    /// </summary>
    /// <param name="eventData">Provided data of the tap event</param>
    public void OnInputDown(InputEventData eventData)
    {
    }

    /// <summary>
    /// This is called if the user ends a tap gesture on the gameobject to which this component is attached
    /// </summary>
    /// <param name="eventData">Provided data of the tap event</param>
    public void OnInputUp(InputEventData eventData)
    {
        // redirect to the specified method
        if (OnPressed != null)
        {
            OnPressed();
        }
    }
}
