using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMenu : MonoBehaviour, ILanguageUpdater
{


    public virtual void OnUpdateLanguage()
    {
    }

    // Use this for initialization
    protected virtual void Start()
    {
        LocalizationManager.Instance.AddUpdateReceiver(this);
    }

    protected virtual void OnDestroy()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.RemoveUpdateReceiver(this);
        }
    }
}
