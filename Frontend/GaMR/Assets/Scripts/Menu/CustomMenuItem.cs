using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Menu))]
public class CustomMenuItem : MonoBehaviour {


    public GameObject menuStyle;
    public List<CustomMenuItem> subMenu;
    [Tooltip("If enabled, the whole menu will be closed and the root menu will be displayed again")]
    public bool closeOnClick;

    private MenuStyleAdapter menuStyleAdapter;
    private GameObject containerInstance;
    private Menu parentMenu;
    private CustomMenuItem parentMenuItem;
    private bool subMenuOpened;

    [Tooltip("Functions which are called if the user clicks the menu entry")]
    public UnityEvent onClickEvent;

    [SerializeField]
    private Texture icon;
    [SerializeField]
    private string text;

    public Texture Icon { get { return icon; } set { menuStyleAdapter.UpdateIcon(value); icon = value; } }

    public string Text { get { return text; } set { menuStyleAdapter.UpdateText(value); text = value; } }

    public void Create(Menu parentMenu, CustomMenuItem parent)
    {
        this.parentMenuItem = parent;
        this.parentMenu = parentMenu;
        subMenuOpened = false;
        containerInstance = Instantiate(menuStyle, parentMenu.transform);

        menuStyleAdapter = containerInstance.GetComponent<MenuStyleAdapter>();
        menuStyleAdapter.Initialize();
        menuStyleAdapter.RegisterForClickEvent(OnClick);
        menuStyleAdapter.UpdateText(text);
        menuStyleAdapter.UpdateIcon(icon);

        if (onClickEvent == null)
        {
            onClickEvent = new UnityEvent();
        }
    }

    public void Destroy()
    {
        GameObject.Destroy(containerInstance);
    }

    public void OnClick()
    {
        Debug.Log("clicked " + text);
        // invoke the defined action
        onClickEvent.Invoke();

        // reset the menu on click if closeOnClick is enabled
        if (closeOnClick)
        {
            parentMenu.ResetMenu();
            return;
        }

        // also spawn the sub menu if it exists
        if (subMenu.Count > 0 && !subMenuOpened)
        {
            //InstantiateSubMenus();
            parentMenu.InstantiateMenu(GameObjectInstance.transform.localPosition, menuStyleAdapter.Size, subMenu, this, true);
            if (parentMenuItem != null)
            {
                parentMenu.HideSiblings(this, parentMenuItem.subMenu);
            }
            else
            {
                parentMenu.HideSiblings(this, parentMenu.rootMenu);
            }
            subMenuOpened = true;
        }
        else if (subMenu.Count > 0)
        {
            // destroy the sub menu
            DestroySubmenus();
            // if parentMenuItem is null => it is the root
            if (parentMenuItem != null)
            {
                parentMenu.ShowSiblings(parentMenuItem.subMenu);
            }
            else
            {
                parentMenu.ShowSiblings(parentMenu.rootMenu);
            }
            subMenuOpened = false;
        }
    }

    private void DestroySubmenus()
    {
        foreach(CustomMenuItem child in subMenu)
        {
            child.Destroy();
        }
    }

    [System.Obsolete("InstantiateSubMenus is obsolte, please use parentMenu.InstantiateMenu instead")]
    private void InstantiateSubMenus()
    {
        Vector3 instantiatePosition = GameObjectInstance.transform.localPosition;
        instantiatePosition.y -= menuStyleAdapter.Size.y + parentMenu.padding;

        // instantiate the menu
        for (int i = 0; i < subMenu.Count; i++)
        {
            subMenu[i].Create(this.parentMenu, this);
            if (i > 0)
            {
                // set the correct position
                if (parentMenu.alignment == Direction.HORIZONTAL)
                {
                    // get to the middle between the previous and the current item
                    instantiatePosition.x += (subMenu[i - 1].MenuSytleAdapter.Size.x + parentMenu.padding) / 2;
                    // get to the center of the current item
                    instantiatePosition.x += (subMenu[i].MenuSytleAdapter.Size.x + parentMenu.padding) / 2;
                }
                else
                {
                    // get to the middle between the previous and the current item
                    instantiatePosition.y -= (subMenu[i - 1].MenuSytleAdapter.Size.y + parentMenu.padding) / 2;
                    // get to the center of the current item
                    instantiatePosition.y -= (subMenu[i].MenuSytleAdapter.Size.y + parentMenu.padding) / 2;
                }
            }
            subMenu[i].Position = instantiatePosition;
        }
    }

    public MenuStyleAdapter MenuSytleAdapter { get { return menuStyleAdapter; } }

    public Vector3 Position { get { return containerInstance.transform.localPosition; } set { containerInstance.transform.localPosition = value; } }

    public GameObject GameObjectInstance { get { return containerInstance; } }
}
