using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Menu : MonoBehaviour
{

    public List<CustomMenuItem> rootMenu;
    public float padding = 0.1f;
    public Direction alignment = Direction.HORIZONTAL;
    [Tooltip("This is only used if alignment is set to GRID")]
    public int itemsInOneLine = 3;
    public GameObject defaultMenuStyle;
    private Dictionary<string, CustomMenuItem> allMenuItems;

    public UnityEvent externalInitialization;
    public bool markOnlyOne;
    private CustomMenuItem markedItem;

    // Use this for initialization
    void Start()
    {
        allMenuItems = new Dictionary<string, CustomMenuItem>();
        FillDictionary(rootMenu);
        InstantiateMenu(Vector3.zero, Vector3.zero, rootMenu, null, false, alignment);
        if (externalInitialization != null)
        {
            externalInitialization.Invoke();
        }
    }

    /// <summary>
    /// creates a dictionary of all menu items with their names as key
    /// </summary>
    /// <param name="menuList">The menu list to add to the dictionary</param>
    private void FillDictionary(List<CustomMenuItem> menuList)
    {
        foreach (CustomMenuItem item in menuList)
        {
            if (!allMenuItems.ContainsKey(item.menuItemName))
            {
                allMenuItems.Add(item.menuItemName, item);
            }
            else
            {
                Debug.LogWarning("There are multiple menu items with the name: " + item.menuItemName + Environment.NewLine + "One or more could not be logged in the dictionary");
            }
            if (item.subMenu != null && item.subMenu.Count > 0)
            {
                FillDictionary(item.subMenu);
            }
        }
    }

    /// <summary>
    /// returns a menu item on the menu by its name
    /// </summary>
    /// <param name="name">The name of the menu item</param>
    /// <returns>the menu item with this name; null if it does not exist</returns>
    public CustomMenuItem GetItem(string name)
    {
        if (allMenuItems.ContainsKey(name))
        {
            return allMenuItems[name];
        }
        else
        {
            return null;
        }
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

    /// <summary>
    /// Hides all siblings
    /// </summary>
    /// <param name="current">The current menu item which should not be hidden</param>
    /// <param name="siblings">The menu item list which should be hidden</param>
    public void HideSiblings(CustomMenuItem current, List<CustomMenuItem> siblings)
    {
        foreach (CustomMenuItem item in siblings)
        {
            if (item != current)
            {
                item.Hide();
            }
        }
    }

    /// <summary>
    /// shows all siblings to a menu item
    /// </summary>
    /// <param name="siblings">The siblings to show</param>
    public void ShowSiblings(List<CustomMenuItem> siblings)
    {
        foreach (CustomMenuItem item in siblings)
        {
            if (item.GameObjectInstance != null)
            {
                item.Show();
            }
        }
    }

    /// <summary>
    /// resets the complete menu and displays the root menu again
    /// </summary>
    public void ResetMenu()
    {
        // the menu object should only contain menu items as children
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        InstantiateMenu(Vector3.zero, Vector3.zero, rootMenu, null, false, alignment);
    }

    public void InstantiateMenu(Vector3 instantiatePosition, Vector3 parentItemSize, List<CustomMenuItem> menu, CustomMenuItem parent, bool isSubMenu, Direction alignment)
    {
        if (externalInitialization != null)
        {
            externalInitialization.Invoke();
        }
        Vector3 origInstantiatePos = instantiatePosition;
        if (isSubMenu)
        {
            instantiatePosition.y -= parentItemSize.y + padding;
        }

        // instantiate the menu
        for (int i = 0; i < menu.Count; i++)
        {
            if (menu[i].visibleTo == PlayerType.ALL || menu[i].visibleTo == InformationManager.instance.playerType)
            {
                menu[i].Create(this, parent);
                if (i > 0)
                {
                    // set the correct position
                    if (alignment == Direction.HORIZONTAL)
                    {
                        // if the previous item was created => move to the next position to place the current item there
                        if (menu[i - 1].MenuSytleAdapter != null)
                        {
                            // get to the middle between the previous and the current item
                            instantiatePosition.x += (menu[i - 1].MenuSytleAdapter.Size.x + padding) / 2;
                            // get to the center of the current item
                            instantiatePosition.x += (menu[i].MenuSytleAdapter.Size.x + padding) / 2;
                        }
                    }
                    else if (alignment == Direction.VERTICAL)
                    {
                        // if the previous item was created => move to the next position to place the current item there
                        if (menu[i - 1].MenuSytleAdapter != null)
                        {
                            // get to the middle between the previous and the current item
                            instantiatePosition.y -= (menu[i - 1].MenuSytleAdapter.Size.y + padding) / 2;
                            // get to the center of the current item
                            instantiatePosition.y -= (menu[i].MenuSytleAdapter.Size.y + padding) / 2;
                        }
                    }
                    else if (alignment == Direction.GRID)
                    {
                        // if the previous item was created => move to the next position to place the current item there
                        if (menu[i - 1].MenuSytleAdapter != null)
                        {
                            // get to the middle between the previous and the current item
                            instantiatePosition.x += (menu[i - 1].MenuSytleAdapter.Size.x + padding) / 2;
                            // get to the center of the current item
                            instantiatePosition.x += (menu[i].MenuSytleAdapter.Size.x + padding) / 2;
                            // if one line is filled => move to next line
                            if (i % itemsInOneLine == 0)
                            {
                                // get to the middle between the previous and the current item
                                if (menu[i - 1].MenuSytleAdapter != null)
                                {
                                    instantiatePosition.y -= (menu[i - 1].MenuSytleAdapter.Size.y + padding) / 2;
                                }
                                // get to the center of the current item
                                instantiatePosition.y -= (menu[i].MenuSytleAdapter.Size.y + padding) / 2;
                                // also reset the x coordinate
                                instantiatePosition.x = origInstantiatePos.x;
                            }
                        }
                    }
                }
                menu[i].Position = instantiatePosition;
            }
        }
    }

    public void UpdateTexts()
    {
        foreach (KeyValuePair<string, CustomMenuItem> item in allMenuItems)
        {
            // update the localization
            item.Value.Text = item.Value.InitialText;
        }
    }

    public void MarkOne(CustomMenuItem item)
    {
        if (markedItem != null)
        {
            markedItem.Marked = false;
        }
        item.Marked = true;
        markedItem = item;
    }
}

public enum Direction
{
    HORIZONTAL, VERTICAL, GRID
}
