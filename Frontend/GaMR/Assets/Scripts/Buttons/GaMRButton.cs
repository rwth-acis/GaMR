using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Registers InputEvents and redirects them to a specified method
/// </summary>
public class GaMRButton : MonoBehaviour, IInputClickHandler
{

    /// <summary>
    /// The method which should be called if the button is pressed
    /// </summary>
    public Action OnPressed;
    public Action<GaMRButton> OnButtonPressed;
    protected string text;
    private TextMesh textMesh;
    protected Sprite icon;
    private SpriteRenderer spriteRenderer;
    private bool visible;

    public int Data { get; set; } // custom data

    /// <summary>
    /// Gets the necessary components of the button
    /// </summary>
    private void Start()
    {
        Transform caption = transform.Find("Caption");
        if (caption != null)
        {
            textMesh = caption.GetComponent<TextMesh>();
        }

        Transform iconTransform = transform.Find("Icon");
        if (iconTransform != null)
        {
            spriteRenderer = iconTransform.GetComponent<SpriteRenderer>();
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

    /// <summary>
    /// The text which is displayed on the button
    /// If changed, the new text will automatically be applied to the button's representation
    /// </summary>
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

    /// <summary>
    /// The icon which is displayed on the button
    /// If changed, the new icon is automatically applied to the button's representation
    /// </summary>
    public Sprite Icon
    {
        get { return icon; }
        set
        {
            icon = value;
            if (spriteRenderer == null)
            {
                Transform iconTransform = transform.Find("Icon");
                if (iconTransform != null)
                {
                    spriteRenderer = iconTransform.GetComponent<SpriteRenderer>();
                }
            }

            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = icon;
            }
        }
    }

    /// <summary>
    /// True if the icon is visible on the button
    /// Changes to this value are automatically applied
    /// </summary>
    public bool IconVisible
    {
        get
        {
            if (spriteRenderer == null)
            {
                Transform iconTransform = transform.Find("Icon");
                if (iconTransform != null)
                {
                    spriteRenderer = iconTransform.GetComponent<SpriteRenderer>();
                }
            }

            if (spriteRenderer != null)
            {
                return spriteRenderer.gameObject.activeSelf;
            }
            else
            {
                // if there is no icon => nothing visible
                return false;
            }
        }
        set
        {
            if (spriteRenderer == null)
            {
                Transform iconTransform = transform.Find("Icon");
                if (iconTransform != null)
                {
                    spriteRenderer = iconTransform.GetComponent<SpriteRenderer>();
                }
            }

            if (spriteRenderer != null)
            {
                spriteRenderer.gameObject.SetActive(value);
            }
        }
    }

    public bool Visible
    {
        get { return visible; }
        set
        {
            visible = value;
            gameObject.SetActive(value);
        }

    }
}
