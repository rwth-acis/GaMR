using HoloToolkit.Unity.InputModule;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongPressButton : GaMRButton, IInputHandler {

    /// <summary>
    /// gets called every frame on a long press
    /// </summary>
    public Action OnLongPressed;

    private float clickedTime = 0;
    private bool pressed = false;

    /// <summary>
    /// Gets called if the user presses down the button
    /// </summary>
    /// <param name="eventData"></param>
    public void OnInputDown(InputEventData eventData)
    {
        pressed = true;
        InputManager.Instance.OverrideFocusedObject = gameObject;
    }

    /// <summary>
    /// Gets called if the user releases the button
    /// Resets the timer
    /// </summary>
    /// <param name="eventData"></param>
    public void OnInputUp(InputEventData eventData)
    {
        pressed = false;
        InputManager.Instance.OverrideFocusedObject = null;
        clickedTime = 0;
    }

    /// <summary>
    /// If the button is pressed, each frame the render time of the frame will be added to the total time that the button has been pressed
    /// If the totaled time passes a threshold, the tap will be recognized as a long press on the button
    /// In this case the specified function OnLongPressed will be executed each frame until the user releases the button
    /// </summary>
    public void Update()
    {
        if (pressed)
        {
            clickedTime += Time.deltaTime;
            if (clickedTime > 1f)
            {
                if (OnLongPressed != null)
                {
                    OnLongPressed();
                }
            }
        }
    }
}
