using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ButtonConfiguration : MonoBehaviour
{
    [SerializeField]
    private ButtonType type;
    [SerializeField]
    private Sprite icon;
    [SerializeField]
    private string caption;
    [SerializeField]
    private bool showIcon = true;
    [SerializeField]
    private bool showCaption = true;

    private SpriteRenderer spriteRenderer;
    private Transform captionTransform;
    private Transform ledTransform;
    private Transform contentTransform;
    private ButtonType lastButtonType;

    private FocusableButton currentButtonComponent;

    private bool firstUpdate = true;

    // stored button settings
    private float pressDepth;

    /// <summary>
    /// makes sure that the script is only affecting the editor
    /// the script is destroyed in play mode to save performance
    /// </summary>
    private void Awake()
    {
#if !UNITY_EDITOR
        
        RemoveScript();
#endif
        if (Application.isEditor && Application.isPlaying)
        {
            RemoveScript();
        }
    }

    private void RemoveScript()
    {
        // destroy all inactive children => they should not be shown and so they can be deleted
        // exception: frame

        Transform frame = transform.Find("Frame");

        foreach (Transform trans in transform)
        {
            if (trans != frame && !trans.gameObject.activeSelf)
            {
                GameObject.Destroy(trans.gameObject);
            }
        }

        Destroy(this); // save performance => the button is already configured in the editor
    }

    private void Initialize()
    {
        Transform spriteTransform = transform.Find("Icon");
        captionTransform = transform.Find("Caption");
        ledTransform = transform.Find("LED");
        contentTransform = transform.Find("Content");

        if (spriteTransform != null)
        {
            spriteRenderer = spriteTransform.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                icon = spriteRenderer.sprite;
            }
        }

        currentButtonComponent = gameObject.GetComponent<FocusableButton>();

        UpdateButtonType();
    }

    private void Update()
    {
        if (firstUpdate)
        {
            Initialize();
            firstUpdate = false;
        }

        // update the assigned icon
        if (spriteRenderer != null)
        {
            spriteRenderer.gameObject.SetActive(showIcon);
            if (spriteRenderer.sprite != icon)
            {
                spriteRenderer.sprite = icon;
            }
        }

        if (captionTransform != null && showCaption != captionTransform.gameObject.activeSelf)
        {
            captionTransform.gameObject.SetActive(showCaption);
        }

        if (lastButtonType != type)
        {
            UpdateButtonType();
            lastButtonType = type;
        }

        if (currentButtonComponent != null)
        {
            currentButtonComponent.Text = caption;
        }
    }

    private void UpdateButtonType()
    {
        // disable all specific controls and just re-enable the needed control
        // this simplifies transition between button types
        DisableAllSpecificControls();
        // remove button script to exchange it for a new type
        // but store the settings of the button
        StoreButtonSettings();
        DestroyImmediate(currentButtonComponent);

        switch (type)
        {
            case ButtonType.BUTTON:
                currentButtonComponent = gameObject.AddComponent<FocusableButton>();
                break;
            case ButtonType.CHECK_BUTTON:
                if (ledTransform != null)
                {
                    ledTransform.gameObject.SetActive(true);
                    currentButtonComponent = gameObject.AddComponent<FocusableCheckButton>();
                }
                else
                {
                    Debug.LogError("Tried to set focusable check button without LED. This is not allowed (" + gameObject.name + ")");
                }
                break;
            case ButtonType.CONTENT_BUTTON:
                if (contentTransform != null)
                {
                    contentTransform.gameObject.SetActive(true);
                    currentButtonComponent = gameObject.AddComponent<FocusableContentButton>();
                }
                else
                {
                    Debug.LogError("Tried to set focusable content button without content label. This is not allowed (" + gameObject.name + ")");
                }
                break;
        }

        RestoreButtonSettings();
    }

    /// <summary>
    /// Disables all controls which are only used by specialized button types
    /// </summary>
    private void DisableAllSpecificControls()
    {
        if (ledTransform != null)
        {
            ledTransform.gameObject.SetActive(false);
        }
        if (contentTransform != null)
        {
            contentTransform.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// stores the settings which have been made on the FocusableButton, FocusableContentButton or FocusableCheckButton
    /// this is useful for the developer as the specific values can be restored when the script is exchanged
    /// </summary>
    private void StoreButtonSettings()
    {
        if (currentButtonComponent != null)
        {
            pressDepth = currentButtonComponent.pressDepth;
        }
    }

    /// <summary>
    /// restores button settings from saved values of a previous button script
    /// </summary>
    private void RestoreButtonSettings()
    {
        if (currentButtonComponent != null)
        {
            currentButtonComponent.pressDepth = pressDepth;
        }
    }

    private enum ButtonType
    {
        BUTTON, CHECK_BUTTON, CONTENT_BUTTON
    }

}
