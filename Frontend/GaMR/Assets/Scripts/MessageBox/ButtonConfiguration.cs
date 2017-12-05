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

    private SpriteRenderer spriteRenderer;

    private bool firstUpdate = true;

    /// <summary>
    /// makes sure that the script is only affecting the editor
    /// the script is destroyed in play mode to save performance
    /// </summary>
    private void Awake()
    {
#if !UNITY_EDITOR
        Destroy(this);
#endif
    }

    private void Initialize()
    {
        Transform spriteTransform = transform.Find("Icon");
        if (spriteTransform != null)
        {
            spriteRenderer = spriteTransform.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                icon = spriteRenderer.sprite;
            }
        }
    }

    private void Update()
    {
        if (firstUpdate)
        {
            Initialize();
            firstUpdate = false;
        }

        // update the assigned icon
        if (spriteRenderer != null && spriteRenderer.sprite != icon)
        {
            spriteRenderer.sprite = icon;
        }
    }

    private enum ButtonType
    {
        BUTTON, CHECK_BUTTON, CONTENT_BUTTON, CUSTOM
    }

}
