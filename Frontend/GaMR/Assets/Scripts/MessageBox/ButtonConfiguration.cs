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
    private bool showIcon = true;
    [SerializeField]
    private bool showCaption = true;

    private SpriteRenderer spriteRenderer;
    private Transform captionTransform;
    private Transform ledTransform;
    private ButtonType lastButtonType;

    private bool firstUpdate = true;

    /// <summary>
    /// makes sure that the script is only affecting the editor
    /// the script is destroyed in play mode to save performance
    /// </summary>
    private void Awake()
    {
#if !UNITY_EDITOR

        // destroy all inactive children => they should not be shown and so they can be deleted
        foreach (Transform trans in transform)
        {
            if (!trans.gameObject.activeSelf)
            {
                GameObject.Destroy(trans);
            }
        }

        Destroy(this); // save performance => the button is already configured in the editor
#endif
    }

    private void Initialize()
    {
        Transform spriteTransform = transform.Find("Icon");
        captionTransform = transform.Find("Caption");
        ledTransform = transform.Find("LED");

        if (spriteTransform != null)
        {
            spriteRenderer = spriteTransform.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                icon = spriteRenderer.sprite;
            }
        }

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
    }

    private void UpdateButtonType()
    {
        // disable all specific controls and just re-enable the needed control
        // this simplifies transition between button types
        DisableAllSpecificControls();


        switch (type)
        {
            case ButtonType.BUTTON:
                break;
            case ButtonType.CHECK_BUTTON:
                if (ledTransform != null)
                {
                    ledTransform.gameObject.SetActive(true);
                }
                break;
            case ButtonType.CONTENT_BUTTON:
                break;
        }
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
    }

    private enum ButtonType
    {
        BUTTON, CHECK_BUTTON, CONTENT_BUTTON
    }

}
