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

    private Transform caption;
    private Transform container;
    private TextMesh textMesh;
    private Transform iconPlane;
    private Renderer textRenderer;
    private Renderer iconRenderer;

    private List<System.Action> clickListeners;


    // this needs to be initialized manually since "start" is not called in time on instantiated objects
    /// <summary>
    /// Initializes the MenuStyleAdapter
    /// </summary>
    public void Initialize()
    {
        // get the components
        container = transform.Find("Container");

        caption = transform.Find("Caption");
        if (caption != null)
        {
            textMesh = caption.GetComponent<TextMesh>();
            textRenderer = caption.GetComponent<Renderer>();
        }

        iconPlane = transform.Find("IconPlane");
        if (iconPlane != null)
        {
            iconRenderer = iconPlane.GetComponent<Renderer>();
        }

        clickListeners = new List<System.Action>();

    }

    /// <summary>
    /// updates the text on the menu item
    /// also scales the menu item to fit the text
    /// </summary>
    /// <param name="newText">The new text to display on the menu item</param>
    public void UpdateText(string newText)
    {
        if (textMesh != null)
        {
            textMesh.text = newText;
            if (container != null)
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

    public void UpdateIcon(Texture newIcon)
    {
        if (iconRenderer != null)
        {
            iconRenderer.material.SetTexture("_MainTexture", newIcon);
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
            return container.transform.localScale;
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

    public void OnFocusEnter()
    {

    }

    public void OnFocusExit()
    {

    }

    /// <summary>
    /// this is called if a drag gesture is recognized
    /// it is used to make the menu item more sensitive to user input
    /// it handles the case where the user taps for too long on the menu item
    /// </summary>
    /// <param name="eventData">Data of the click event</param>
    public void OnNavigationStarted(NavigationEventData eventData)
    {
        foreach (Action action in clickListeners)
        {
            action();
        }
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
