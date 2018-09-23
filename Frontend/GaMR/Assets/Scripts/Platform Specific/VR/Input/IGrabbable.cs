using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IGrabbable : IEventSystemHandler
{
    /// <summary>
    /// Called if a grab action is started
    /// </summary>
    /// <param name="sender">The WearObjects script on the controller which initiated the grab action</param>
    void OnGrabStarted(VRInputManager sender);

    /// <summary>
    /// Called if a grab action is completed
    /// </summary>
    /// <param name="sender">The WearObjects script on the controller which initiated the event</param>
    void OnGrabCompleted(VRInputManager sender);

    /// <summary>
    /// Called every frame as long as a grab action on the object is going on
    /// </summary>
    /// <param name="sender">The WearObjects script on the controller which initiated the event</param>
    void OnGrabUpdate(VRInputManager sender);
}
