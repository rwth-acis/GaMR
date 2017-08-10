using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the logic for the wait cursor
/// </summary>
public class WaitCursor : Singleton<WaitCursor>
{

    private static int numberOfWaitActions = 0;

    /// <summary>
    /// Initializes the singleton instance and disables the wait cursor
    /// </summary>
    void Start()
    {
        Instance.gameObject.SetActive(false);
    }

    /// <summary>
    /// Displays the wait cursor and adds one to the number of actions which still need the wait cursor
    /// </summary>
    public static void Show()
    {
        if (Instance != null)
        {
            numberOfWaitActions++;
            SetWaitCursorVisiblity();
        }
    }

    /// <summary>
    /// Substracts one from the number of actions which still need the wait cursor and hides the wait cursor if none still need it
    /// </summary>
    public static void Hide()
    {
        if (Instance != null)
        {
            numberOfWaitActions--;
            // just for stability: if the numberOfWaitActions gets smaller than 0 => reset to 0
            if (numberOfWaitActions < 0)
            {
                numberOfWaitActions = 0;
            }
            SetWaitCursorVisiblity();
        }
    }

    /// <summary>
    /// Determines the visibility based on the number of actions which still need the wait cursor: if no action needs a wait cursor anymore => disable it
    /// </summary>
    private static void SetWaitCursorVisiblity()
    {
        if (numberOfWaitActions > 0)
        {
            Instance.gameObject.SetActive(true);
        }
        else
        {
            Instance.gameObject.SetActive(false);
        }
    }
}
