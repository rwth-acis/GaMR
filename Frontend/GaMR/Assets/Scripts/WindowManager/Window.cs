using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour
{
    public bool stackable;
    public bool typeSingleton;
    public string typeId;

    private bool started = false;

    private float windowDepth;
    private IWindow externalWindowLogic;
    private bool overwriteRotation;
    private Vector3 windowNormal;

    private SimpleTagalong tagalong;
    private FaceCamera faceCamera;

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

    public bool OverwriteRotation
    {
        get { return overwriteRotation; }
        set
        {
            if (!value)
            {
                Debug.Log("Unset");
            }
            overwriteRotation = value;
            faceCamera.enabled = !overwriteRotation;
        }
    }

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
        started = true;
    }

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

    private void OnDestroy()
    {
        NotifyWindowManager();
    }

    private void OnDisable()
    {
        NotifyWindowManager();
    }

    private void OnEnable()
    {
        if (started)
        {
            WindowManager.Instance.Add(this);
        }
    }

    private void NotifyWindowManager()
    {
        // if this is called when the scene is unloaded => Instance is null and it does not matter
        if (WindowManager.Instance != null)
        {
            WindowManager.Instance.Remove(this);
        }
    }

    private void Update()
    {
        if (overwriteRotation)
        {
            transform.rotation = Quaternion.LookRotation(-windowNormal);
        }
    }
}
