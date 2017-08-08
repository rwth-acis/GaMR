using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WindowManager : Singleton<WindowManager>
{
    private List<Window> openWindows;

    public float focusedDepth = 2f;
    private float originalFocusedDepth;
    private bool overwriteRotation;
    private Vector3 windowNormal;

    private int layerMask;


    // Use this for initialization
    void Start()
    {
        openWindows = new List<Window>();
        // ignore IgnoreRaycast
        int ignoreRaycast = 1 << 2;
        int ignoreWindows = 1 << 8;

        layerMask = ignoreRaycast | ignoreWindows;
        layerMask = ~layerMask;


        originalFocusedDepth = focusedDepth;
    }

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

    private void UpdateAlignment()
    {
        for (int i = 0; i < openWindows.Count; i++)
        {
            openWindows[i].WindowDepth = focusedDepth + (openWindows.Count - i - 1) * 0.1f;
            openWindows[i].WindowNormal = windowNormal;
            openWindows[i].OverwriteRotation = overwriteRotation;
        }
    }

    public void Remove(Window window)
    {
        openWindows.Remove(window);
        UpdateAlignment();
    }

    private List<Window> CopyWindows()
    {
        List<Window> res = new List<Window>();
        foreach (Window w in openWindows)
        {
            res.Add(w);
        }

        return res;
    }

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
