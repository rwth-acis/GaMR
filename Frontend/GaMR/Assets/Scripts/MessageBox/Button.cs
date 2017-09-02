using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Registers InputEvents and redirects them to a specified method
/// </summary>
public class Button : MonoBehaviour, IInputClickHandler {

    /// <summary>
    /// The method which should be called if the button is pressed
    /// </summary>
    public Action OnPressed;

    /// <summary>
    /// Gets called if the user taps on the button
    /// Executes OnPressed
    /// </summary>
    /// <param name="eventData"></param>
    public virtual void OnInputClicked(InputClickedEventData eventData)
    {
        // redirect to the specified method
        if (OnPressed != null)
        {
            OnPressed();
        }
    }
}
