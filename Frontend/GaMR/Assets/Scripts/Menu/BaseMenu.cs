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

    private void OnDestroy()
    {
        LocalizationManager.Instance.RemoveUpdateReceiver(this);
    }
}
