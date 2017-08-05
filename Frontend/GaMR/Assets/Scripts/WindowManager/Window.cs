using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour
{
    public bool stackable;
    public bool singletonType;
    public string typeId;

    private float windowDepth;
    private IWindow externalWindowLogic;

    private SimpleTagalong tagalong;

    public float WindowDepth
    {
        get { return windowDepth; }
        set { windowDepth = value; tagalong.TagalongDistance = windowDepth; }
    }

    /// <summary>
    /// this is called when the component is created => register it in the window manager
    /// </summary>
    public void Start()
    {
        WindowManager.Instance.Add(this);
        externalWindowLogic = GetComponent<IWindow>();
        tagalong = GetComponent<SimpleTagalong>();
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
        WindowManager.Instance.Remove(this);
    }
}
