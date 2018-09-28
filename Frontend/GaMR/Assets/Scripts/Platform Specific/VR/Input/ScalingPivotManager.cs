using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalingPivotManager : Singleton<ScalingPivotManager>
{
    public Transform controller1;
    public Transform controller2;

    public GameObject Pivot { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Pivot = new GameObject("Scaling Pivot");
        Pivot.transform.parent = transform;
    }

    private void Update()
    {
        if (Pivot != null)
        {
            Pivot.transform.position = Vector3.Lerp(controller1.position, controller2.position, 0.5f);
            Pivot.transform.rotation = Quaternion.LookRotation(controller2.position - controller1.position, Vector3.up);
        }
    }
}
