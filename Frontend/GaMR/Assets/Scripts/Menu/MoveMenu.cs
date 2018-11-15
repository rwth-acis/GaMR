using HoloToolkit.Unity.InputModule;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveMenu : MonoBehaviour, IFocusable, IManipulationHandler
{
    public Vector3 realForward;

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
        InputManager.Instance.OverrideFocusedObject = null;
    }

    public void OnManipulationCompleted(ManipulationEventData eventData)
    {
        InputManager.Instance.OverrideFocusedObject = null;
    }

    public void OnManipulationStarted(ManipulationEventData eventData)
    {
        globalParent = GetGlobalParent();
        startingPoint = globalParent.position;

        InputManager.Instance.OverrideFocusedObject = gameObject;
    }

    public void OnManipulationUpdated(ManipulationEventData eventData)
    {
        if (globalParent == null)
        {
            globalParent = transform;
        }

        Vector3 vectorToCam = Camera.main.transform.position - globalParent.transform.position;

        float speedFactor = vectorToCam.magnitude;
        globalParent.position = startingPoint + speedFactor * eventData.CumulativeDelta;

        FaceUser(vectorToCam);
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

    private void FaceUser(Vector3 vectorToCam)
    {
        globalParent.transform.rotation = Quaternion.LookRotation(vectorToCam, Vector3.up);
        globalParent.transform.rotation = Quaternion.LookRotation(globalParent.transform.TransformDirection(realForward));
    }
}
