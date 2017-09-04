using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : BaseMenu
{

    [SerializeField]
    private GameObject settingsMenu;
    private FocusableButton settingsButton;
    private FocusableButton loginButton;
    private FocusableCheckButton authorButton;
    private bool menuEnabled = true;


    public static Vector3 LastPosition
    {
        get; private set;
    }

    public static Quaternion LastRotation
    {
        get; private set;
    }

    public bool MenuEnabled
    {
        get { return menuEnabled; }
        set
        {
            menuEnabled = value;
            settingsButton.ButtonEnabled = value;
            loginButton.ButtonEnabled = value;
            authorButton.ButtonEnabled = value;
        }
    }


    protected override void Start()
    {
        base.Start();
        InitializeButtons();
    }

    private void InitializeButtons()
    {
        settingsButton = transform.Find("Settings Button").gameObject.AddComponent<FocusableButton>();
        loginButton = transform.Find("Login Button").gameObject.AddComponent<FocusableButton>();
        authorButton = transform.Find("Author Button").gameObject.AddComponent<FocusableCheckButton>();

        settingsButton.OnPressed = ShowSettings;
        loginButton.OnPressed = Login;
        authorButton.OnPressed = TogglePlayerType;

        authorButton.ButtonChecked = (InformationManager.Instance.playerType == PlayerType.AUTHOR);

        OnUpdateLanguage();
    }

    private void TogglePlayerType()
    {
        if (authorButton.ButtonChecked)
        {
            InformationManager.Instance.playerType = PlayerType.AUTHOR;
        }
        else
        {
            InformationManager.Instance.playerType = PlayerType.STUDENT;
        }
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

    public override void OnUpdateLanguage()
    {
        settingsButton.Text = LocalizationManager.Instance.ResolveString("Settings");
        loginButton.Text = LocalizationManager.Instance.ResolveString("Login");
        authorButton.Text = LocalizationManager.Instance.ResolveString("Author");
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        LastPosition = transform.parent.position;
        LastRotation = transform.parent.rotation;
    }
}
