using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VRInputManager : Tool
{
    public LayerMask pointableLayers;
    public LayerMask grabbableLayers;

    private Transform lastHitTransform;
    private Transform lastDown;

    private LaserPointer laserPointer;
    private BowTeleport teleport;

    private bool pointerEnabled;
    private bool pointerEnabledBeforeTeleport;

    private GameObject grabbedObject;
    private bool isGrabbing;
    private GameObject collidingObject;

    private static List<VRInputManager> instances = new List<VRInputManager>();

    public bool PointerEnabled
    {
        get
        {
            return pointerEnabled;
        }
        set
        {
            if (value && pointerEnabled != value) // if switched on: switch all others off
            {
                foreach(VRInputManager instance in instances)
                {
                    if (instance != this)
                    {
                        instance.PointerEnabled = false;
                    }
                }
            }
            pointerEnabled = value;
            laserPointer.enabled = value;
            laserPointer.mask = pointableLayers;
        }
    }


    private void Start()
    {
        laserPointer = GetComponent<LaserPointer>();
        teleport = GetComponent<BowTeleport>();
        instances.Add(this);
        PointerEnabled = true;
    }

    private void Update()
    {
        // teleport starting
        if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            teleport.ShowTeleportBow = true;
            pointerEnabledBeforeTeleport = pointerEnabled;
            PointerEnabled = false;
        }

        // teleport finished
        if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            teleport.ShowTeleportBow = false;
            PointerEnabled = pointerEnabledBeforeTeleport;
        }

        // toggle pointer
        if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            PointerEnabled = !PointerEnabled;
        }

        if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            if (!isGrabbing && collidingObject != null)
            {
                isGrabbing = ExecuteEvents.Execute<IGrabbable>(collidingObject, null, (x, y) => x.OnGrabStarted(this));
                if (isGrabbing)
                {
                    grabbedObject = collidingObject;
                    Debug.Log("grabbed " + grabbedObject);
                }
            }
        }

        Debug.Log("isGrabbing: " + isGrabbing);

        // if object is grabbed
        if (isGrabbing)
        {
            // update grab if trigger is still held
            if (Controller.GetPress(SteamVR_Controller.ButtonMask.Trigger))
            {
                ExecuteEvents.Execute<IGrabbable>(grabbedObject, null, (x, y) => x.OnGrabUpdate(this));
            }
            // if trigger is released  => grab stops
            else if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                ExecuteEvents.Execute<IGrabbable>(grabbedObject, null, (x, y) => x.OnGrabCompleted(this));
                isGrabbing = false;
                Debug.Log("released " + grabbedObject);
                grabbedObject = null;
            }
        }

        // update pointer
        if (pointerEnabled)
        {
            PointerUpdate();
        }
    }

    private void PointerUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100, pointableLayers))
        {
            InputEventData inputEventData = new InputEventData(EventSystem.current);

            if (lastHitTransform != hit.transform)
            {
                if (!Controller.GetPress(SteamVR_Controller.ButtonMask.Trigger))
                {
                    if (lastHitTransform != null)
                    {
                        ExecuteEvents.Execute<IFocusable>(lastHitTransform.gameObject, null, (x, y) => x.OnFocusExit());
                    }
                    ExecuteEvents.Execute<IFocusable>(hit.transform.gameObject, null, (x, y) => x.OnFocusEnter());
                }
                else
                {
                    if (lastHitTransform != null && lastHitTransform == lastDown)
                    {
                        ExecuteEvents.Execute<IFocusable>(lastHitTransform.gameObject, null, (x, y) => x.OnFocusExit());
                    }
                    if (hit.transform == lastDown)
                    {
                        ExecuteEvents.Execute<IFocusable>(hit.transform.gameObject, null, (x, y) => x.OnFocusEnter());
                        ExecuteEvents.Execute<IInputHandler>(hit.transform.gameObject, null, (x, y) => x.OnInputDown(inputEventData));
                    }
                }
                lastHitTransform = hit.transform;
            }

            if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                ExecuteEvents.Execute<IInputHandler>(hit.transform.gameObject, null, (x, y) => x.OnInputDown(inputEventData));
                lastDown = hit.transform;
            }
            if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Trigger) && lastDown == hit.transform)
            {
                ExecuteEvents.Execute<IInputHandler>(hit.transform.gameObject, null, (x, y) => x.OnInputUp(inputEventData));

                // raise OnInputClicked
                InputClickedEventData inputClickedEventData = new InputClickedEventData(EventSystem.current);
                inputClickedEventData.Initialize(null, 0, 0, InteractionSourcePressInfo.Select);
                ExecuteEvents.Execute<IInputClickHandler>(hit.transform.gameObject, null, (x, y) => x.OnInputClicked(inputClickedEventData));
                lastDown = null;
            }
        }
        else
        {
            if (lastHitTransform != null)
            {
                ExecuteEvents.Execute<IFocusable>(lastHitTransform.gameObject, null, (x, y) => x.OnFocusExit());
                lastHitTransform = null;
            }
            lastDown = null;
        }
    }

    private void SetCollidingObject(Collider col)
    {
        if (collidingObject != null || col.gameObject.GetComponent<Rigidbody>() == null)
        {
            return;
        }
        collidingObject = col.gameObject;
    }

    ///// <summary>
    ///// Can be called by other scripts to set an object as grabbed
    ///// </summary>
    ///// <param name="newObject"></param>
    //public void GrabObject(GameObject newObject)
    //{
    //    ReleaseObject(); // in case anything is already in the hand: release it (this is needed if the function is called by another script)
    //    if (grabbedObject == null)
    //    {
    //        isGrabbing = ExecuteEvents.Execute<IGrabbable>(newObject, null, (x, y) => x.OnGrabStarted(this)); // only returns true if a grabbable object was found
    //        if (isGrabbing)
    //        {
    //            grabbedObject = newObject;
    //        }
    //    }
    //}

    //public void ReleaseObject()
    //{
    //    if (grabbedObject != null)
    //    {
    //        ExecuteEvents.Execute<IGrabbable>(grabbedObject, null, (x, y) => x.OnGrabCompleted(this));
    //        grabbedObject = null;
    //        isGrabbing = false;
    //    }
    //}

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    private void OnTriggerEnter(Collider other)
    {
        SetCollidingObject(other);
    }

    private void OnTriggerExit(Collider other)
    {
        collidingObject = null;
    }

    private void OnTriggerStay(Collider other)
    {
        SetCollidingObject(other);
    }
}
