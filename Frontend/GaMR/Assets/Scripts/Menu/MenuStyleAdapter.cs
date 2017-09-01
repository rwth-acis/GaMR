using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// MenuStyleAdapter is placed on the container of a menu item
/// this container has a text and/or a plane with a texture as a child
/// </summary>
public class MenuStyleAdapter : MonoBehaviour, IInputClickHandler, INavigationHandler, IFocusable
{
    [Tooltip("The offset of the container border to the text")]
    public Vector2 offset;

    public bool adaptToContent = true;

    private Transform caption;
    private Transform container;
    private TextMesh textMesh;
    private Transform icon;
    private Renderer textRenderer;
    private Renderer iconRenderer;
    private Renderer containerRenderer;
    private bool itemEnabled;
    private Color enabledColor;
    protected bool marked;

    public Color disabledColor = new Color(0.7f, 0.7f, 0.7f);
    public Color markedColor = Color.red;
    public Color focusedColor = Color.blue;

    protected List<System.Action> clickListeners;

    /// <summary>
    /// whether or not the item is (logically) enabled
    /// </summary>
    public bool ItemEnabled
    {
        get { return itemEnabled; }
        set { itemEnabled = value; SetState(value); }
    }

    /// <summary>
    /// whether or not the item is marked
    /// </summary>
    public bool Marked
    {
        get { return marked; }
        set { marked = value; UpdateContainerColor(); }
    }

    // this needs to be initialized manually since "start" is not called in time on instantiated objects
    /// <summary>
    /// Initializes the MenuStyleAdapter
    /// </summary>
    public virtual void Initialize()
    {
        // get the components
        container = transform.Find("Container");

        caption = transform.Find("Caption");
        if (caption != null)
        {
            textMesh = caption.GetComponent<TextMesh>();
            textRenderer = caption.GetComponent<Renderer>();
        }

        icon = transform.Find("Icon");
        if (icon != null)
        {
            iconRenderer = icon.GetComponent<Renderer>();
        }

        if (container != null)
        {
            containerRenderer = container.GetComponent<Renderer>();
            enabledColor = containerRenderer.material.color;
        }

        clickListeners = new List<System.Action>();

    }

    /// <summary>
    /// (logically) enables or disables the menu item
    /// </summary>
    /// <param name="enabled"></param>
    private void SetState(bool enabled)
    {
        if (containerRenderer != null)
        {
            if (enabled)
            {
                containerRenderer.material.color = enabledColor;
            }
            else
            {
                containerRenderer.material.color = disabledColor;
            }
        }
    }

    /// <summary>
    /// updates the text on the menu item
    /// also scales the menu item to fit the text
    /// </summary>
    /// <param name="newText">The new text to display on the menu item</param>
    public virtual void UpdateText(string newText)
    {
        if (textMesh != null)
        {
            textMesh.text = newText;
            if (container != null && adaptToContent)
            {
                Quaternion oldRotation = textRenderer.transform.rotation;
                textRenderer.transform.rotation = Quaternion.identity;
                container.transform.localScale = new Vector3(textRenderer.bounds.size.x + offset.x, textRenderer.bounds.size.y + offset.y, container.transform.localScale.z);
                //container.transform.localScale = new Vector3(
                //    (textRenderer.bounds.size.x + offset.x) / parentMenu.transform.localScale.x,
                //    (textRenderer.bounds.size.y + offset.y) / parentMenu.transform.localScale.y,
                //    container.transform.localScale.z);
                textRenderer.transform.rotation = oldRotation;
            }
        }

    }

    /// <summary>
    /// sets the icon texture on the iconRenderer-object
    /// </summary>
    /// <param name="newIcon">the icon to set</param>
    public virtual void UpdateIcon(Texture newIcon)
    {
        if (iconRenderer != null)
        {
            if (newIcon != null)
            {
                iconRenderer.gameObject.SetActive(true);
                iconRenderer.material.SetTexture("_MainTex", newIcon);
            }
            else
            {
                iconRenderer.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// the size of the menu item
    /// (which is the same as the size of the container)
    /// the container should include all content of the menu item
    /// </summary>
    public Vector3 Size
    {
        get
        {
            Bounds bounds = Geometry.GetBoundsIndependentFromRotation(container);
            return bounds.size;
        }
    }

    /// <summary>
    /// registers a method so that it is called if the menu item is clicked
    /// </summary>
    /// <param name="action">The action to execute if a click happens</param>
    public void RegisterForClickEvent(System.Action action)
    {
        clickListeners.Add(action);
    }

    /// <summary>
    /// called when a click happened
    /// calls all methods which are registered
    /// </summary>
    /// <param name="eventData">Data of the click event</param>
    public void OnInputClicked(InputClickedEventData eventData)
    {
        foreach (Action action in clickListeners)
        {
            action();
        }
    }

    /// <summary>
    /// called when the user focuses the item
    /// </summary>
    public virtual void OnFocusEnter()
    {
        if (itemEnabled)
        {
            containerRenderer.material.color = focusedColor;
        }
    }

    /// <summary>
    /// called when the user stops focusing the item
    /// </summary>
    public virtual void OnFocusExit()
    {
        UpdateContainerColor();
    }

    /// <summary>
    /// sets the container color according to its current state
    /// </summary>
    public void UpdateContainerColor()
    {
        if (itemEnabled)
        {
            if (marked)
            {
                containerRenderer.material.color = markedColor;
            }
            else
            {
                containerRenderer.material.color = enabledColor;
            }
        }
        else
        {
            containerRenderer.material.color = disabledColor;
        }
    }

    /// <summary>
    /// this is called if a drag gesture is recognized
    /// it is used to make the menu item more sensitive to user input
    /// it handles the case where the user taps for too long on the menu item
    /// </summary>
    /// <param name="eventData">Data of the click event</param>
    public void OnNavigationStarted(NavigationEventData eventData)
    {
        //foreach (Action action in clickListeners)
        //        {
        //            action();
        //        }
    }

    public void OnNavigationUpdated(NavigationEventData eventData)
    {
    }

    public void OnNavigationCompleted(NavigationEventData eventData)
    {
    }

    public void OnNavigationCanceled(NavigationEventData eventData)
    {
    }
}
