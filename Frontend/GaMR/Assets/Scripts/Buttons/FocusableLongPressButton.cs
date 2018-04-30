using HoloToolkit.Unity.InputModule;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusableLongPressButton : FocusableButton, IHoldHandler
{

    /// <summary>
    /// gets called every frame on a long press
    /// </summary>
    public Action OnLongPressed;

    private bool longPressed = false;

    /// <summary>
    /// Is called if the user cancels holding the button
    /// </summary>
    /// <param name="eventData"></param>
    public void OnHoldCanceled(HoldEventData eventData)
    {
        longPressed = false;
    }

    /// <summary>
    /// Is called if the user releases the button after holding it down
    /// </summary>
    /// <param name="eventData"></param>
    public void OnHoldCompleted(HoldEventData eventData)
    {
        longPressed = false;
    }

    /// <summary>
    /// Is called if the user holds the button
    /// </summary>
    /// <param name="eventData"></param>
    public void OnHoldStarted(HoldEventData eventData)
    {
        longPressed = true;
    }

    /// <summary>
    /// If the button is pressed, each frame the specified function OnLongPressed
    /// will be executed until the user releases the button again
    /// </summary>
    public void Update()
    {
        if (longPressed)
        {
            if (OnLongPressed != null)
            {
                OnLongPressed();
            }
        }
    }
}
