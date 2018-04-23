using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseInputManager : Singleton<MouseInputManager>
{
    [Tooltip("Amount of time that the mouse is held down until a drag gesture is recognized")]
    public float timeUntilHold = 0.5f;
    public float distanceUntilDrag = 0.03f;
    public Transform cursor;
    public LayerMask layerMask;

    private GameObject lastFocused = null;
    private float timeSinceDown = 0f;
    private GameObject selectedObject;
    private float distanceToSelectedObject;
    private Vector3 mouseStartPosition;
    private bool isDrag = false;

    public Vector3 HitPosition { get; private set; }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 10, layerMask))
        {
            HitPosition = hit.point;
            TransformCursor(hit.point, hit.normal);
            Transform objectHit = hit.transform;

            // check if focus behavior event needs to be raised
            // this should only happen if the mouse is not pressed
            if (!isDrag && lastFocused != objectHit.gameObject)
            {
                ExecuteEvents.Execute<IFocusable>(lastFocused, null, (x, y) => x.OnFocusExit());
                ExecuteEvents.Execute<IFocusable>(objectHit.gameObject, null, (x, y) => x.OnFocusEnter());
                lastFocused = objectHit.gameObject;
            }

            InputEventData inputEventData = new InputEventData(EventSystem.current);

            if (Input.GetMouseButtonDown(0))
            {
                ExecuteEvents.Execute<IInputHandler>(objectHit.gameObject, null, (x, y) => x.OnInputDown(inputEventData));
                selectedObject = objectHit.gameObject; // down started on this selected object
                distanceToSelectedObject = hit.distance;
                timeSinceDown = 0f;
                mouseStartPosition = Input.mousePosition;
                isDrag = false;
            }
            if (Input.GetMouseButtonUp(0))
            {
                // in any case: fire input up
                ExecuteEvents.Execute<IInputHandler>(objectHit.gameObject, null, (x, y) => x.OnInputUp(inputEventData));
                if (isDrag)
                {
                    RaiseManipulationComplete();
                }
                else
                {
                    if (timeSinceDown < timeUntilHold)
                    {
                        // raise OnInputClicked
                        InputClickedEventData inputClickedEventData = new InputClickedEventData(EventSystem.current);
                        inputClickedEventData.Initialize(null, 0, 0, InteractionSourcePressInfo.Select);
                        ExecuteEvents.Execute<IInputClickHandler>(objectHit.gameObject, null, (x, y) => x.OnInputClicked(inputClickedEventData));
                    }
                }


                isDrag = false; // deactivate drag operation
            }
            if (Input.GetMouseButton(0))
            {
                // count how long the mouse has been pushed to detect mouse holding
                timeSinceDown += Time.deltaTime;
                // if no drag manipulation yet => find out if the user is dragging the mouse now
                if (!isDrag)
                {
                    Vector3 cummulativeDelta = CalculateDiff(mouseStartPosition, Input.mousePosition, distanceToSelectedObject);
                    if (cummulativeDelta.magnitude > distanceUntilDrag)
                    {
                        // toggle drag operation and also invoke the manipulation started event
                        isDrag = true;
                        RaiseManipulationStart(cummulativeDelta);
                    }
                }

                // if a dragging operation is active => perform the drag
                if (isDrag)
                {
                    RaiseManipulationUpdate();
                }
            }
        }
        else // no object hit
        {
            HitPosition = Vector3.zero;
            Vector3 cursorPos = Input.mousePosition;
            cursorPos.z = 5;
            cursorPos = Camera.main.ScreenToWorldPoint(cursorPos);
            TransformCursor(cursorPos, -ray.direction);

            // no raycast hit
            // raise a focus exit event on last focused object if there is one => mouse just left it
            // only do this if no drag movement is active
            if (!isDrag && lastFocused != null)
            {
                ExecuteEvents.Execute<IFocusable>(lastFocused, null, (x, y) => x.OnFocusExit());
                lastFocused = null;
            }
            if (isDrag) // consider drag movements even if the mouse is not on the object anymore
            {
                if (Input.GetMouseButtonUp(0)) //  still listen to stop the manipulation
                {
                    RaiseManipulationComplete();
                    isDrag = false;
                }
                else // resume a drag operation, even if the mosue left the object's collider
                {
                    RaiseManipulationUpdate();
                }                
            }
        }
    }

    private Vector3 CalculateDiff(Vector3 initialPosition, Vector3 currentPosition, float distance)
    {
        initialPosition = MousePosToWorld(initialPosition, distance);
        currentPosition = MousePosToWorld(currentPosition, distance);

        return currentPosition - initialPosition;
    }

    private Vector3 MousePosToWorld(Vector3 input, float distance)
    {
        input.z = distanceToSelectedObject;
        input = Camera.main.ScreenToWorldPoint(input);
        return input;
    }

    private void RaiseManipulationUpdate()
    {
        Vector3 cummulativeDelta = CalculateDiff(mouseStartPosition, Input.mousePosition, distanceToSelectedObject);
        ManipulationEventData data = new ManipulationEventData(EventSystem.current);
        data.Initialize(null, 0, null, cummulativeDelta);
        ExecuteEvents.Execute<IManipulationHandler>(selectedObject.gameObject, null, (x, y) => x.OnManipulationUpdated(data));
    }

    private void RaiseManipulationStart(Vector3 cummulativeDelta)
    {
        ManipulationEventData data = new ManipulationEventData(EventSystem.current);
        data.Initialize(null, 0, null, cummulativeDelta);
        ExecuteEvents.Execute<IManipulationHandler>(selectedObject.gameObject, null, (x, y) => x.OnManipulationStarted(data));
    }

    private void RaiseManipulationComplete()
    {
        Vector3 cummulativeDelta = CalculateDiff(mouseStartPosition, Input.mousePosition, distanceToSelectedObject);
        ManipulationEventData data = new ManipulationEventData(EventSystem.current);
        data.Initialize(null, 0, null, cummulativeDelta);
        ExecuteEvents.Execute<IManipulationHandler>(selectedObject.gameObject, null, (x, y) => x.OnManipulationCompleted(data));
    }

    private void TransformCursor(Vector3 position, Vector3 normal)
    {
        if (cursor != null)
        {
            cursor.position = position;
            Quaternion rotation = new Quaternion();
            rotation.SetLookRotation(-normal);
            cursor.rotation = rotation;
        }
    }


}
