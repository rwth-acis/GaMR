﻿using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Registers drag movements and propagates them to the TransformationManager of the object which should be transformed
/// </summary>
public class DragHandle : MonoBehaviour, /*INavigationHandler,*/ IManipulationHandler
{
    [Tooltip("Defines which operation this handle initiates: Translation, Rotation or Scale")]
    public HandleType handleType;
    [Tooltip("The object to manipulate")]
    public Transform toManipulate;
    private TransformationManager transformationManager;
    [Tooltip("For rotation: defines the rotation axis")]
    public Vector3 gestureOrientation;
    [Tooltip("The speed of the operation")]
    public float speed = 0.01f;

    private Vector3 lastCummulativeDelta, upVector, forwardVector, rightVector;

    private Vector3 currentAxis, currentAxisProjection, projectionCross;

    /// <summary>
    /// Gets the value which has the maximum absolute value
    /// e.g. for -1, 10, -500 it returns -500
    /// </summary>
    /// <param name="values">Array with the values to check</param>
    /// <returns>The value with the maximum absolute value</returns>
    private float GetMaxAbsolute(float[] values)
    {
        float max;
        float realValue;

        max = Math.Abs(values[0]);
        realValue = values[0];

        for (int i = 1; i < values.Length; i++)
        {
            if (Math.Abs(values[i]) > max)
            {
                max = Math.Abs(values[i]);
                realValue = values[i];
            }
        }

        return realValue;
    }

    /// <summary>
    /// Gets the necessary components: the transformationManager and  the inputManager
    /// </summary>
    void Start()
    {
        transformationManager = toManipulate.GetComponent<TransformationManager>();
    }

    /// <summary>
    /// Called if the user starts a manipulation gesture
    /// </summary>
    /// <param name="eventData">The data of the manipulation event</param>
    public void OnManipulationStarted(ManipulationEventData eventData)
    {
        // set the focus to this object so that the manipulation continues to have effect even if
        // the user does not look at the gameObject anymore
        InputManager.Instance.OverrideFocusedObject = gameObject;

        lastCummulativeDelta = Vector3.zero;
        upVector = toManipulate.up;
        forwardVector = toManipulate.forward;
        rightVector = toManipulate.right;

        if (gestureOrientation == Vector3.up)
        {
            currentAxis = toManipulate.up;
        }
        else if (gestureOrientation == Vector3.right)
        {
            currentAxis = toManipulate.right;
        }
        else if (gestureOrientation == Vector3.forward)
        {
            currentAxis = toManipulate.forward;
        }

        currentAxisProjection = Vector3.ProjectOnPlane(currentAxis, Camera.main.transform.forward);

        projectionCross = Vector3.Cross(Camera.main.transform.forward, currentAxisProjection);

        Debug.Log("Current Axis: " + currentAxis);
        Debug.Log("Projection: " + currentAxisProjection);
        Debug.Log("Projection Cross: " + projectionCross);
    }

    /// <summary>
    /// Called when a manipulation gesture is currently happening
    /// </summary>
    /// <param name="eventData">The data of the manipulation event</param>
    public void OnManipulationUpdated(ManipulationEventData eventData)
    {
        if (transformationManager.enabled)
        {
            switch (handleType)
            {
                case HandleType.SCALE:
                    {
                        // it is also possible to scale without preserving the aspect ratio
                        // this option is implemented in here but it is currently not used
                        bool preserveAspectRatio = true;
                        if (preserveAspectRatio)
                        {
                            Vector2 centerProj = Camera.main.WorldToScreenPoint(toManipulate.position);
                            Vector2 handleProj = Camera.main.WorldToScreenPoint(transform.position);

                            Vector2 fromCenterToHandle = handleProj - centerProj;
                            fromCenterToHandle = fromCenterToHandle.normalized;

                            int drawDirection = Math.Sign(Vector2.Dot(eventData.CumulativeDelta, fromCenterToHandle));
                            // fromCenterToHandle points outwards from the center
                            // thus:
                            // if drawDirection < 0: inwards-drag => scale down
                            // if drawDirection > 0: outwards-drag => scale up

                            // just get the most dominant 2D-axis to determine the strength of the scale
                            float max = Math.Max(Math.Abs(eventData.CumulativeDelta.x), Math.Abs(eventData.CumulativeDelta.y));

                            // determine scaling factor
                            float scaleFac = 1.0f + (speed * max * drawDirection);

                            // scale
                            transformationManager.Scale(scaleFac * Vector3.one);
                        }
                        else
                        {
                            Vector3 scaleVec = speed * new Vector3(
                    eventData.CumulativeDelta.x * gestureOrientation.x,
                    eventData.CumulativeDelta.y * gestureOrientation.y,
                    eventData.CumulativeDelta.z * gestureOrientation.z);
                            transformationManager.Scale(scaleVec);
                        }
                        break;
                    }
                case HandleType.ROTATE:
                    {
                        //float[] values = new[] {eventData.CumulativeDelta.x,
                        //    eventData.CumulativeDelta.y, eventData.CumulativeDelta.z};
                        //float rotationValue = GetMaxAbsolute(values);

                        Vector3 delta = lastCummulativeDelta - eventData.CumulativeDelta;
                        // the x axis of the delta is flipped => correct this
                        delta = new Vector3(-delta.x, delta.y, delta.z);

                        Vector3 projectedDelta = Vector3.Project(delta, projectionCross);

                        float rotationAngle = projectedDelta.magnitude * Vector3.Dot(projectedDelta.normalized, projectionCross.normalized) * 360;

                        // the following are debug lines which can be used to visualize the relevant vectors
                        // they are only visible in the game view
                        //Debug.DrawLine(toManipulate.position, toManipulate.position + currentAxis, Color.red);
                        //Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.position + currentAxisProjection, Color.magenta);
                        //Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.position + projectionCross, Color.green);
                        //Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.position + (projectedDelta * 10000), Color.blue);
                        //Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.position + (delta * 10000), Color.cyan);


#if ORIG_SOLUTION
                        Vector3 objToCam = Camera.main.transform.position - toManipulate.position;
                        Vector3 objToTarget = (Camera.main.transform.position + delta) - toManipulate.position;
                        float rotationAngle = Vector3.Angle(objToCam, objToTarget);                  
                        Debug.Log("Gesture orientation: " + gestureOrientation);

                        Vector3 crossProduct = Vector3.Cross(objToCam, objToTarget);
                        if (gestureOrientation == Vector3.up)
                        {
                            if (Vector3.Dot(crossProduct, upVector) < 0)
                            {
                                rotationAngle *= -1;
                            }

                            //if (Vector3.Dot(upVector, Vector3.up) > -0.5f  && Vector3.Dot(upVector, Vector3.up) < 0.5f)
                            //{
                            //    rotationAngle *= -1;
                            //}
                        }
                        else if (gestureOrientation == Vector3.right)
                        {
                            if (Vector3.Dot(crossProduct, rightVector) > 0)
                            {
                                rotationAngle *= -1;
                            }

                            //if (Vector3.Dot(rightVector, Vector3.right) > -0.5f && Vector3.Dot(rightVector, Vector3.right) < 0.5f)
                            //{
                            //    rotationAngle *= -1;
                            //}
                        }
                        else if (gestureOrientation == Vector3.forward)
                        {
                            if (Vector3.Dot(crossProduct, forwardVector) > 0)
                            {
                                rotationAngle *= -1;
                            }

                            //if (Vector3.Dot(forwardVector, Vector3.forward) > -0.5f && Vector3.Dot(forwardVector, Vector3.forward) < 0.5f)
                            //{
                            //    rotationAngle *= -1;
                            //}
                        }

#endif

                        Debug.Log("Rotation by: " + rotationAngle);
                        //Debug.Log("Cum. Delta: " + eventData.CumulativeDelta);
                        lastCummulativeDelta = eventData.CumulativeDelta;

                        transformationManager.Rotate(gestureOrientation, speed * rotationAngle);
                        break;
                    }
                case HandleType.TRANSLATE:
                    {
                        //Vector3 translationVec = new Vector3(speed * eventData.CumulativeDelta.x, speed * eventData.CumulativeDelta.y, speed * eventData.CumulativeDelta.z);
                        transformationManager.Translate(speed * eventData.CumulativeDelta);
                        break;
                    }

            }
        }
    }

    /// <summary>
    /// Called when the user successfully completes a manipulation gesture
    /// </summary>
    /// <param name="eventData">The data of the manipulation event</param>
    public void OnManipulationCompleted(ManipulationEventData eventData)
    {
        // release the focus-override so that other objects can now be affected by the manipulation gesture
        InputManager.Instance.OverrideFocusedObject = null;
    }

    /// <summary>
    /// Called when the user cancels a manipulation gesture
    /// </summary>
    /// <param name="eventData">The data of the manipulation gesture</param>
    public void OnManipulationCanceled(ManipulationEventData eventData)
    {
        // release the focus-override so that other objects can now be affected by the manipulation gesture
        InputManager.Instance.OverrideFocusedObject = null;
    }
}

/// <summary>
/// The type of transformation that the handle is responsible for
/// </summary>
public enum HandleType
{
    SCALE, ROTATE, TRANSLATE
}
