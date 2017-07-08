using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingBoxMenuInitialization : MonoBehaviour
{
    Menu menu;
    AttachementManager attachementManager;

    // Use this for initialization
    void Start()
    {
        menu = transform.Find("MenuCenter/Menu").GetComponent<Menu>();
        attachementManager = GetComponentInChildren<AttachementManager>();
    }


    public void InitializeMenu()
    {
        CustomMenuItem toggleEditMode = menu.GetItem("ToggleEditMode");
        if (attachementManager.IsQuiz && InformationManager.instance.playerType == PlayerType.STUDENT)
        {
            toggleEditMode.ItemEnabled = false;
        }
        else
        {
            toggleEditMode.ItemEnabled = true;
        }
    }
}
