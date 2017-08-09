using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Window manager which handles all open windows and positions them
/// </summary>
public class WindowManager : Singleton<WindowManager>
{
    private List<Window> openWindows;

    public float focusedDepth = 2f;
    private float originalFocusedDepth;
    private bool overwriteRotation;
    private Vector3 windowNormal;

    private int layerMask;


    /// <summary>
    /// initializes the raycast mask
    /// </summary>
    void Start()
    {
        openWindows = new List<Window>();
        // ignore IgnoreRaycast
        int ignoreRaycast = 1 << 2;
        // ignore Window
        int ignoreWindows = 1 << 8;

        // combine
        layerMask = ignoreRaycast | ignoreWindows;
        // invert to ignore the set layers and check all others
        layerMask = ~layerMask;


        originalFocusedDepth = focusedDepth;
    }

    /// <summary>
    /// Adds the specified window to the list of open windows
    /// </summary>
    /// <param name="window">The new window</param>
    public void Add(Window window)
    {
        // if it is stackable => close all open windows
        if (!window.stackable)
        {
            List<Window> tempCopy = CopyWindows();
            foreach (Window w in tempCopy)
            {
                w.Close();
            }
        }
        // if it is a type singleton => close all windows of the same type
        else if (window.typeSingleton)
        {
            List<Window> tempCopy = CopyWindows();
            foreach (Window w in tempCopy)
            {
                if (w.typeId == window.typeId)
                {
                    w.Close();
                }
            }
        }
        // else: just add the window to the stack

        openWindows.Add(window);

        UpdateAlignment();
    }

    /// <summary>
    /// Updates the positions and rotations of all open windows according to their position in the openWindows-stack
    /// </summary>
    private void UpdateAlignment()
    {
        for (int i = 0; i < openWindows.Count; i++)
        {
            openWindows[i].WindowDepth = focusedDepth + (openWindows.Count - i - 1) * 0.1f;
            openWindows[i].WindowNormal = windowNormal;
            openWindows[i].OverwriteRotation = overwriteRotation;
        }
    }

    /// <summary>
    /// Removes a window from the openWindows-stack
    /// </summary>
    /// <param name="window">The window to remove</param>
    public void Remove(Window window)
    {
        openWindows.Remove(window);
        UpdateAlignment();
    }

    /// <summary>
    /// Helper method to create a deep copy of the openWindows-stack
    /// </summary>
    /// <returns></returns>
    private List<Window> CopyWindows()
    {
        List<Window> res = new List<Window>();
        foreach (Window w in openWindows)
        {
            res.Add(w);
        }

        return res;
    }

    /// <summary>
    /// Periodically creates a raycast in order to check if the window is placed inside an obstacle
    /// Adapts the depth of the windows if a obstacle was hit and also adapts their rotation to the obstacles normal
    /// </summary>
    private void FixedUpdate()
    {
        if (openWindows.Count > 0)
        {
            Vector3 direction = openWindows[openWindows.Count - 1].transform.position - Camera.main.transform.position;

            RaycastHit hit;


            if (Physics.Raycast(Camera.main.transform.position, direction, out hit, 2f, layerMask))
            {
                focusedDepth = hit.distance - 0.1f;
                overwriteRotation = true;
                windowNormal = hit.normal;


                if (focusedDepth < Camera.main.nearClipPlane && !UIMessage.Instance.Active)
                {
                    UIMessage.Instance.Show(LocalizationManager.Instance.ResolveString("Your are too close to an obstacle. Step away so that windows can be displayed."));
                }
                else if (UIMessage.Instance.Active && focusedDepth >= Camera.main.nearClipPlane)
                {
                    UIMessage.Instance.Hide();
                }
            }
            else
            {
                focusedDepth = originalFocusedDepth;
                overwriteRotation = false;
            }

            UpdateAlignment();
        }
        else
        {
            UIMessage.Instance.Hide();
        }
    }
}
