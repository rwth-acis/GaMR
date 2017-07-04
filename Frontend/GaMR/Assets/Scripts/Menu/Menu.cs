using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour {

    public List<CustomMenuItem> rootMenu;
    public float padding = 0.1f;
    public Direction alignment = Direction.HORIZONTAL;
    [Tooltip("This is only used if alignment is set to GRID")]
    public int itemsInOneLine = 3;
    public GameObject defaultMenuStyle;

	// Use this for initialization
	void Start () {
        //InitMenu();
        InstantiateMenu(Vector3.zero, Vector3.zero, rootMenu, null, false);
    }

    [System.Obsolete("InitMenu is obsolte, please use InstantiateMenu instead")]
    private void InitMenu()
    {
        Vector3 instantiatePosition = new Vector3(0, 0, 0);

        // instantiate the menu
        for (int i = 0; i < rootMenu.Count; i++)
        {
            rootMenu[i].Create(this, null);
            if (i > 0)
            {
                // set the correct position
                if (alignment == Direction.HORIZONTAL)
                {
                    // get to the middle between the previous and the current item
                    instantiatePosition.x += (rootMenu[i - 1].MenuSytleAdapter.Size.x + padding) / 2;
                    // get to the center of the current item
                    instantiatePosition.x += (rootMenu[i].MenuSytleAdapter.Size.x + padding) / 2;
                }
                else if (alignment == Direction.VERTICAL)
                {
                    // get to the middle between the previous and the current item
                    instantiatePosition.y -= (rootMenu[i - 1].MenuSytleAdapter.Size.y + padding) / 2;
                    // get to the center of the current item
                    instantiatePosition.y -= (rootMenu[i].MenuSytleAdapter.Size.y + padding) / 2;
                }
            }
            rootMenu[i].Position = instantiatePosition;
        }
    }

    public void HideSiblings(CustomMenuItem current, List<CustomMenuItem> siblings)
    {
        foreach(CustomMenuItem item in siblings)
        {
            if (item != current)
            {
                item.GameObjectInstance.SetActive(false);
            }
        }
    }

    public void ShowSiblings(List<CustomMenuItem> siblings)
    {
        foreach (CustomMenuItem item in siblings)
        {
            item.GameObjectInstance.SetActive(true);
        }
    }

    /// <summary>
    /// resets the complete menu and displays the root menu again
    /// </summary>
    public void ResetMenu()
    {
        // the menu object should only contain menu items as children
        foreach(Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        InstantiateMenu(Vector3.zero, Vector3.zero, rootMenu, null, false);
    }

    public void InstantiateMenu(Vector3 instantiatePosition, Vector3 parentItemSize, List<CustomMenuItem> menu, CustomMenuItem parent, bool isSubMenu)
    {
        Vector3 origInstantiatePos = instantiatePosition;
        if (isSubMenu)
        {
            instantiatePosition.y -= parentItemSize.y + padding;
        }

        // instantiate the menu
        for (int i = 0; i < menu.Count; i++)
        {
            menu[i].Create(this, parent);
            if (i > 0)
            {
                // set the correct position
                if (alignment == Direction.HORIZONTAL)
                {
                    // get to the middle between the previous and the current item
                    instantiatePosition.x += (menu[i - 1].MenuSytleAdapter.Size.x + padding) / 2;
                    // get to the center of the current item
                    instantiatePosition.x += (menu[i].MenuSytleAdapter.Size.x + padding) / 2;
                }
                else if (alignment == Direction.VERTICAL)
                {
                    // get to the middle between the previous and the current item
                    instantiatePosition.y -= (menu[i - 1].MenuSytleAdapter.Size.y + padding) / 2;
                    // get to the center of the current item
                    instantiatePosition.y -= (menu[i].MenuSytleAdapter.Size.y + padding) / 2;
                }
                else if (alignment == Direction.GRID)
                {
                    // get to the middle between the previous and the current item
                    instantiatePosition.x += (menu[i - 1].MenuSytleAdapter.Size.x + padding) / 2;
                    // get to the center of the current item
                    instantiatePosition.x += (menu[i].MenuSytleAdapter.Size.x + padding) / 2;
                    // if one line is filled => move to next line
                    if (i % itemsInOneLine == 0)
                    {
                        // get to the middle between the previous and the current item
                        instantiatePosition.y -= (menu[i - 1].MenuSytleAdapter.Size.y + padding) / 2;
                        // get to the center of the current item
                        instantiatePosition.y -= (menu[i].MenuSytleAdapter.Size.y + padding) / 2;
                        // also reset the x coordinate
                        instantiatePosition.x = origInstantiatePos.x;
                    }
                }
            }
            menu[i].Position = instantiatePosition;
        }
    }
}

public enum Direction
{
    HORIZONTAL, VERTICAL, GRID
}
