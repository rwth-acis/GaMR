using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Registers InputEvents and redirects them to a specified method
/// </summary>
public class Button : MonoBehaviour, IInputClickHandler
{

    /// <summary>
    /// The method which should be called if the button is pressed
    /// </summary>
    public Action OnPressed;
    public Action<Button> OnButtonPressed;
    protected string text;
    private TextMesh textMesh;

    public int Data { get; set; } // custom data

    private void Start()
    {
        Transform caption = transform.Find("Caption");
        if (caption != null)
        {
            textMesh = caption.GetComponent<TextMesh>();
        }
    }

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

        if (OnButtonPressed != null)
        {
            OnButtonPressed(this);
        }
    }

    public string Text
    {
        get { return text; }
        set
        {
            text = value;
            if (textMesh == null)
            {
                Transform caption = transform.Find("Caption");
                if (caption != null)
                {
                    textMesh = caption.GetComponent<TextMesh>();
                }
            }

            if (textMesh != null)
            {
                textMesh.text = text;
            }
        }
    }
}
