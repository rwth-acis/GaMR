using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitCursor : MonoBehaviour
{
    private static WaitCursor instance;

    // Use this for initialization
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        gameObject.SetActive(false);
    }

    public static void Show()
    {
        if (instance != null)
        {
            instance.gameObject.SetActive(true);
        }
    }

    public static void Hide()
    {
        if (instance != null)
        {
            instance.gameObject.SetActive(false);
        }
    }
}
