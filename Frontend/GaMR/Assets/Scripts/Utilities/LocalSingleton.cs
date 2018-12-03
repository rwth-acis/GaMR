using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalSingleton<T> : MonoBehaviour where T : LocalSingleton<T>
{
    public static T Instance
    {
        get;private set;
    }

    protected virtual void Awake()
    {
        if (Instance != null)
        {
            if (Instance != this)
            {
                Destroy(this);
                Debug.LogWarning("Destroyed second singleton instance of " + gameObject.name);
            }
        }
        else
        {
            Instance = (T)this;
        }
    }

    protected virtual void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
