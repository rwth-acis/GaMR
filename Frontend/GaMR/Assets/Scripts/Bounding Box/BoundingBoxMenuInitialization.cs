using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// initializes and maniuplates the menu on the bounding box
/// </summary>
public class BoundingBoxMenuInitialization : MonoBehaviour
{
    Menu menu;
    AttachementManager attachementManager;

    /// <summary>
    /// fetches the menu of the bounding box and the attachement manager
    /// </summary>
    void Start()
    {
        menu = transform.Find("MenuCenter/Menu").GetComponent<Menu>();
        attachementManager = GetComponentInChildren<AttachementManager>();
    }

    /// <summary>
    /// enables or disables menu items on the bounding box depending on the application state
    /// </summary>
    public void InitializeMenu()
    {
        CustomMenuItem toggleEditMode = menu.GetItem("ToggleEditMode");
        if (attachementManager.IsQuiz && InformationManager.Instance.playerType == PlayerType.STUDENT)
        {
            toggleEditMode.ItemEnabled = false;
        }
        else
        {
            toggleEditMode.ItemEnabled = true;
        }

        CustomMenuItem loadAnnotations = menu.GetItem("LoadAnnotations");
        if (!attachementManager.IsQuiz)
        {
            loadAnnotations.ItemEnabled = false;
        }
        else
        {
            loadAnnotations.ItemEnabled = true;
        }
    }
}
