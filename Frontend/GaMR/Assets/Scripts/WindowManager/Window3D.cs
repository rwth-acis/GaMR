using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Component marks the gameobject as a window
/// and handles window behavior
/// </summary>
public class Window3D : MonoBehaviour
{
    [Tooltip("Can the window be stacked on top of existing windows?")]
    public bool stackable;
    [Tooltip("If enabled, only one window of this type can exist")]
    public bool typeSingleton;
    [Tooltip("The type of the window")]
    public string typeId;

    private bool started = false;

    private float windowDepth;
    private IWindow externalWindowLogic;
    private bool overwriteRotation;
    private Vector3 windowNormal;

    private SimpleTagalong tagalong;
    private FaceCamera faceCamera;

    private Renderer[] renderers;

    /// <summary>
    /// The depth of the window
    /// </summary>
    public float WindowDepth
    {
        get { return windowDepth; }
        set
        {
            windowDepth = value;
            if (tagalong == null)
            {
                tagalong = GetComponent<SimpleTagalong>();
            }
            tagalong.TagalongDistance = windowDepth;
        }
    }

    /// <summary>
    /// If true: the window will use WindowNormal to determine its rotation
    /// if false: the window will get its rotation from the FaceCamera script
    /// </summary>
    public bool OverwriteRotation
    {
        get { return overwriteRotation; }
        set
        {
            overwriteRotation = value;
            faceCamera.enabled = !overwriteRotation;
        }
    }

    /// <summary>
    /// The normal vector of the window plane
    /// It is applied to the window if OverwriteRotation is true
    /// </summary>
    public Vector3 WindowNormal
    {
        get { return windowNormal; }
        set
        {
            windowNormal = value;
        }
    }

    /// <summary>
    /// this is called when the component is created => register it in the window manager
    /// </summary>
    public void Start()
    {
        externalWindowLogic = GetComponent<IWindow>();
        tagalong = GetComponent<SimpleTagalong>();
        faceCamera = GetComponent<FaceCamera>();
        WindowManager.Instance.Add(this);
        renderers = GetComponentsInChildren<Renderer>();
        started = true;
    }

    /// <summary>
    /// closes the window
    /// If a custom close method was specified using IWindow IWindow.CloseWindow() will be executed
    /// Else: the window will be destroyed
    /// </summary>
    public void Close()
    {
        if (externalWindowLogic != null)
        {
            externalWindowLogic.CloseWindow();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// called if the window is destroyed
    /// </summary>
    private void OnDestroy()
    {
        NotifyWindowManager();
    }

    /// <summary>
    /// called if the window is disabled (closed)
    /// </summary>
    private void OnDisable()
    {
        NotifyWindowManager();
    }

    /// <summary>
    /// called if the window is enabled (opened)
    /// </summary>
    private void OnEnable()
    {
        if (started)
        {
            WindowManager.Instance.Add(this);
        }
    }

    /// <summary>
    /// Notifies the window manager that this window is closed
    /// </summary>
    private void NotifyWindowManager()
    {
        // if this is called when the scene is unloaded => Instance is null and it does not matter
        if (WindowManager.Instance != null)
        {
            WindowManager.Instance.Remove(this);
        }
    }

    /// <summary>
    /// Updates the rotation if overwriteRotation is enabled
    /// </summary>
    private void Update()
    {
        if (overwriteRotation)
        {
            transform.rotation = Quaternion.LookRotation(-windowNormal);
        }
    }
}
