using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour, IWindow
{
    public GameObject menuObject;

    private static GameObject staticMenuObject;
    private static GameObject instance;
    private static Menu menu;

    public void Start()
    {
        // initialize for static call
        staticMenuObject = menuObject;
        Show();
    }

    public static void Show()
    {
        if (instance == null)
        {
            instance = GameObject.Instantiate(staticMenuObject);
            menu = instance.GetComponent<Menu>();
        }
        else
        {
            instance.SetActive(true);
        }
        instance.transform.position = Camera.main.transform.position + new Vector3(-0.2f,0,0) + 2 * Camera.main.transform.forward;
    }

    public static void Close()
    {
        if (instance != null)
        {
            menu.ResetMenu();
            instance.SetActive(false);
        }
    }

    public void CloseWindow()
    {
        Close();
    }

    public static GameObject Instance
    {
        get { return instance; }
    }
}
