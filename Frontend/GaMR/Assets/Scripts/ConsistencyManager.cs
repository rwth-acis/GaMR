using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsistencyManager : Singleton<ConsistencyManager>
{
    protected override void Awake()
    {
        // there should only be one gameobject in the scene and it is consistent across all scenes
        // if the instance is already set => destroy this gameobject if it is not the instance
        if (Instance != null)
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        base.Awake();
    }
}
