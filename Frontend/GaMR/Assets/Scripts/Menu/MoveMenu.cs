using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMenu : MonoBehaviour, IFocusable, IManipulationHandler
{
    public Vector3 rotationOffset;

    private Vector3 startingPoint;
    private Transform globalParent;

    public void OnFocusEnter()
    {

    }

    public void OnFocusExit()
    {
    }

    public void OnManipulationCanceled(ManipulationEventData eventData)
    {
    }

    public void OnManipulationCompleted(ManipulationEventData eventData)
    {
    }

    public void OnManipulationStarted(ManipulationEventData eventData)
    {
        globalParent = GetGlobalParent();
        startingPoint = transform.position;
    }

    public void OnManipulationUpdated(ManipulationEventData eventData)
    {
        if (globalParent == null)
        {
            globalParent = transform;
        }
        globalParent.position = startingPoint + eventData.CumulativeDelta;

        globalParent.transform.LookAt(Camera.main.transform);
        globalParent.transform.eulerAngles += rotationOffset;
    }

    private Transform GetGlobalParent()
    {
        Transform current = transform;
        while (current.parent != null)
        {
            current = current.parent;
        }

        return current;
    }
}
