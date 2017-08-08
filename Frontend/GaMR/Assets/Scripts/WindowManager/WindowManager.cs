using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : Singleton<WindowManager>
{
    private List<Window> openWindows;

    public float focusedDepth = 2f;
    private float originalFocusedDepth;
    private bool overrideRotation;
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
        Vector3 camForward = Camera.main.transform.forward;

        RaycastHit hit;


        if (Physics.Raycast(Camera.main.transform.position, camForward, out hit, 2f, layerMask))
        {
            focusedDepth = hit.distance - 0.1f;



            if (focusedDepth < Camera.main.nearClipPlane && !UIMessage.Instance.Active && openWindows.Count > 0)
            {
                UIMessage.Instance.Show(LocalizationManager.Instance.ResolveString("Your are too close to an obstacle. Step away so that windows can be displayed."));
            }
            else if (UIMessage.Instance.Active && (focusedDepth >= Camera.main.nearClipPlane || openWindows.Count == 0))
            {
                UIMessage.Instance.Hide();
            }
            UpdateAlignment();
        }
        else
        {
            focusedDepth = originalFocusedDepth;
        }
    }
}
