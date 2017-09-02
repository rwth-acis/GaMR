using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FocusableButton : Button, IFocusable, IInputHandler
{
    private GameObject focusHighlight;
    private const float pressDepth = 0.008f;
    private bool pressed;
    private Renderer rend;
    private bool buttonEnabled = true;
    private Color defaultColor;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        defaultColor = rend.material.color;
    }

    public GameObject FocusHighlight
    {
        get { return focusHighlight; }
        set
        {
            focusHighlight = value;
            OnFocusExit();
        }
    }

    public bool ButtonEnabled
    {
        get { return buttonEnabled; }
        set
        {
            if (value)
            {
                rend.material.color = defaultColor;
            }
            else
            {
                OnFocusExit();
                rend.material.color = new Color(0.9f,0.9f,0.9f);
            }
            buttonEnabled = value;
        }
    }

    public void OnFocusEnter()
    {
        if (buttonEnabled)
        {
            if (focusHighlight != null)
            {
                focusHighlight.SetActive(true);
            }
        }
    }

    public void OnFocusExit()
    {
        if (buttonEnabled)
        {
            if (focusHighlight != null)
            {
                focusHighlight.SetActive(false);
            }
            ButtonUp();
        }
    }

    private void ButtonDown()
    {
        if (!pressed && buttonEnabled)
        {
            transform.localPosition = new Vector3(
                transform.localPosition.x + pressDepth,
                transform.localPosition.y,
                transform.localPosition.z
                );
            pressed = true;
        }
    }

    private void ButtonUp()
    {
        if (pressed && buttonEnabled)
        {
            transform.localPosition = new Vector3(
                transform.localPosition.x - pressDepth,
                transform.localPosition.y,
                transform.localPosition.z
                );
            pressed = false;
        }
    }

    public void OnInputDown(InputEventData eventData)
    {
        ButtonDown();
    }

    public void OnInputUp(InputEventData eventData)
    {
        ButtonUp();
    }

    public override void OnInputClicked(InputClickedEventData eventData)
    {
        if (buttonEnabled)
        {
            base.OnInputClicked(eventData);
        }
    }
}
