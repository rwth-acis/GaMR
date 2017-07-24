using HoloToolkit.Unity.InputModule;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongPressButton : Button, IInputHandler {

    /// <summary>
    /// gets called every frame on a long press
    /// </summary>
    public Action OnLongPressed;

    private float clickedTime = 0;
    private bool pressed = false;

    public void OnInputDown(InputEventData eventData)
    {
        pressed = true;
    }

    public void OnInputUp(InputEventData eventData)
    {
        pressed = false;
        clickedTime = 0;
    }

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
