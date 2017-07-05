using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

/// <summary>
/// Notfies listeners about input events
/// </summary>
public class TapNotifier : MonoBehaviour, IInputHandler
{

    private UnityEvent inputDownEvent;
    private UnityEvent inputUpEvent;

    /// <summary>
    /// calls the initialization for the events
    /// </summary>
    public void Start()
    {
        Init();
    }

    /// <summary>
    /// initializes the events
    /// </summary>
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

    /// <summary>
    /// Adds a new listener for the input-down-event
    /// </summary>
    /// <param name="callback"></param>
    public void RegisterListenerOnInputDown(UnityAction callback)
    {
        Init();
        if (callback != null)
        {
            inputDownEvent.AddListener(callback);
        }
    }

    /// <summary>
    /// adds a new listener for the input-up-event
    /// </summary>
    /// <param name="callback"></param>
    public void RegisterListenerOnInputUp(UnityAction callback)
    {
        Init();
        if (callback != null)
        {
            inputUpEvent.AddListener(callback);
        }
    }

    /// <summary>
    /// nofiy all listeners that the input-down-event occured
    /// </summary>
    /// <param name="eventData"></param>
    public void OnInputDown(InputEventData eventData)
    {
        if (inputDownEvent != null)
        {
            inputDownEvent.Invoke();
        }
    }

    /// <summary>
    /// nofiy all listeners that the input-up-event occured
    /// </summary>
    /// <param name="eventData"></param>
    public void OnInputUp(InputEventData eventData)
    {
        if (inputUpEvent != null)
        {
            inputUpEvent.Invoke();
        }
    }
}
