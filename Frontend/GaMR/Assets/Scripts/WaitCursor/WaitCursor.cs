using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the logic for the wait cursor
/// </summary>
public class WaitCursor : Singleton<WaitCursor>
{

    /// <summary>
    /// Initializes the singleton instance and disables the wait cursor
    /// </summary>
    void Start()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Displays the wait cursor
    /// </summary>
    public static void Show()
    {
        if (Instance != null)
        {
            Instance.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Hides the wait cursor
    /// </summary>
    public static void Hide()
    {
        if (Instance != null)
        {
            Instance.gameObject.SetActive(false);
        }
    }
}
