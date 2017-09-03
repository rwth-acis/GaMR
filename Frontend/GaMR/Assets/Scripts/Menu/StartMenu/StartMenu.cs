﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{

    [SerializeField]
    private GameObject settingsMenu;
    private FocusableButton settingsButton;
    private FocusableButton loginButton;
    private bool menuEnabled = true;

    public bool MenuEnabled
    {
        get { return menuEnabled; }
        set
        {
            menuEnabled = value;
            settingsButton.ButtonEnabled = value;
            loginButton.ButtonEnabled = value;
        }
    }


    private void Start()
    {
        InitializeButtons();
    }

    private void InitializeButtons()
    {
        settingsButton = transform.Find("Settings Button").gameObject.AddComponent<FocusableButton>();
        loginButton = transform.Find("Login Button").gameObject.AddComponent<FocusableButton>();

        settingsButton.OnPressed = ShowSettings;
        loginButton.OnPressed = Login;

        settingsButton.Text = LocalizationManager.Instance.ResolveString("Settings");
        loginButton.Text = LocalizationManager.Instance.ResolveString("Login");
    }

    private void Login()
    {
        AuthorizationManager.Instance.Login();
    }

    private void ShowSettings()
    {
        GameObject settingsInstance = Instantiate(settingsMenu);
        SettingsMenu settings = settingsInstance.GetComponent<SettingsMenu>();
        settings.OnCloseAction = () =>
        {
            MenuEnabled = true;
        };
        settingsInstance.transform.parent = transform;
        settingsInstance.transform.rotation = transform.rotation;
        settingsInstance.transform.localPosition = Vector3.zero;
        settingsInstance.transform.Translate(new Vector3(-0.1f, 0.01f, 0.01f));
        MenuEnabled = false;

    }
}
