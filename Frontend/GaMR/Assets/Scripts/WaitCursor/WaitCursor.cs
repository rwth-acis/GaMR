using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the logic for the wait cursor
/// </summary>
public class WaitCursor : MonoBehaviour
{
    /// <summary>
    /// the singleton instance of the wait cursor which should be manipulated
    /// </summary>
    private static WaitCursor instance;

    /// <summary>
    /// Initializes the singleton instance and disables the wait cursor
    /// </summary>
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Displays the wait cursor
    /// </summary>
    public static void Show()
    {
        if (instance != null)
        {
            instance.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Hides the wait cursor
    /// </summary>
    public static void Hide()
    {
        if (instance != null)
        {
            instance.gameObject.SetActive(false);
        }
    }
}
