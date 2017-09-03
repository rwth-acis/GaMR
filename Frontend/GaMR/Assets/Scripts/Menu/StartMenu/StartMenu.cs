using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{

    private FocusableButton settingsButton;
    private FocusableButton loginButton;
    private bool menuEnabled;

    public bool MenuEnabled
    {
        get { return menuEnabled; }
        set
        {
            menuEnabled = value;
            settingsButton.ButtonEnabled = false;
            loginButton.ButtonEnabled = false;
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
        throw new NotImplementedException();
    }
}
