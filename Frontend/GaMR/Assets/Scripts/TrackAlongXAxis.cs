using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackAlongXAxis : MonoBehaviour {

    public float distance = 3f;
    private GazeManager gazeManager;
    private Transform parent;

    public void Start()
    {
        gazeManager = ComponentGetter.GetComponentOnGameobject<GazeManager>("InputManager");
        parent = transform.parent;
    }

    public void Update()
    {
        Vector3 cursorRelativeToKeyboard = parent.InverseTransformPoint(gazeManager.HitPosition);

        transform.localPosition = new Vector3(
            cursorRelativeToKeyboard.x,
            transform.localPosition.y,
            transform.localPosition.z);
    }
}
