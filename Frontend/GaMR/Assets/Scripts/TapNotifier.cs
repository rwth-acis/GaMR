using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class TapNotifier : MonoBehaviour, IInputHandler
{

    private UnityEvent inputDownEvent;
    private UnityEvent inputUpEvent;


    public void Start()
    {
        Init();
    }

    private void Init()
    {
        if (inputDownEvent == null)
        {
            inputDownEvent = new UnityEvent();
        }

        if (inputUpEvent == null)
        {
            inputUpEvent = new UnityEvent();
        }
    }

    public void RegisterListenerOnInputDown(UnityAction callback)
    {
        Init();
        if (callback != null)
        {
            inputDownEvent.AddListener(callback);
        }
    }

    public void RegisterListenerOnInputUp(Action callback)
    {
        if (inputUpEvent != null)
        {
            inputUpEvent.Invoke();
        }
    }

    public void OnInputDown(InputEventData eventData)
    {
        if (inputDownEvent != null)
        {
            inputDownEvent.Invoke();
        }
    }

    public void OnInputUp(InputEventData eventData)
    {
    }
}
