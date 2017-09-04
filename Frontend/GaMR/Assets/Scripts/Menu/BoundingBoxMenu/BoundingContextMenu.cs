using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingContextMenu : BaseMenu
{
    FocusableButton button;
    BoundingBoxMenu boundingBoxMenu;

    protected override void Start()
    {
        base.Start();
        boundingBoxMenu = transform.parent.Find("Bounding Box Menu").GetComponent<BoundingBoxMenu>();
        InitializeButtons();
    }

    private void InitializeButtons()
    {
        button = transform.Find("Button").gameObject.AddComponent<FocusableButton>();

        boundingBoxMenu.OnCloseAction = () =>
        {
            gameObject.SetActive(true);
        };

        button.OnPressed = () =>
        {
            boundingBoxMenu.gameObject.SetActive(true);
            gameObject.SetActive(false);
        };


    }
}
