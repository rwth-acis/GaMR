using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackAlongXAxis : MonoBehaviour {

    public float distance = 3f;
    private GazeManager gazeManager;
    private Transform parent;

    // the velocity at which the transform is currently moving to its target position
    private Vector3 velocity = Vector3.zero;
    // the time it takes to reach the target position
    public float smoothTime = 0.3f;

    public void Start()
    {
        gazeManager = ComponentGetter.GetComponentOnGameobject<GazeManager>("InputManager");
        parent = transform.parent;
    }

    public void Update()
    {
        Vector3 cursorRelativeToKeyboard = parent.InverseTransformPoint(gazeManager.HitPosition);

        Vector3 targetPos = new Vector3(
            cursorRelativeToKeyboard.x,
            transform.localPosition.y,
            transform.localPosition.z);

        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPos, ref velocity, smoothTime);
    }
}
