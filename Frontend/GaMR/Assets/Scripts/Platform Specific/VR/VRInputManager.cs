using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VRInputManager : Tool
{
    private Transform lastHitTransform;
    private Transform lastDown;

    private void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100))
        {
            if (lastHitTransform != hit.transform)
            {
                if (lastHitTransform != null)
                {
                    ExecuteEvents.Execute<IFocusable>(lastHitTransform.gameObject, null, (x, y) => x.OnFocusExit());
                }
                ExecuteEvents.Execute<IFocusable>(hit.transform.gameObject, null, (x, y) => x.OnFocusEnter());
                lastHitTransform = hit.transform;
            }

            InputEventData inputEventData = new InputEventData(EventSystem.current);

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
        }
    }
}
