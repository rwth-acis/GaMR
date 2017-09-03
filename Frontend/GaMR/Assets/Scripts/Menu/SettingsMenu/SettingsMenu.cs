using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject languageMenu;

    FocusableButton closeButton;
    FocusableContentButton languageButton;
    FocusableContentButton modelServerButton;
    FocusableContentButton gamificationServerButton;
    FocusableContentButton sharingServerButton;
    FocusableContentButton collisionButton;

    private bool menuEnabled = true;

    public Action OnCloseAction
    { get; set; }

    public bool MenuEnabled
    {
        get { return menuEnabled; }
        set
        {
            menuEnabled = value;
            closeButton.ButtonEnabled = value;
            languageButton.ButtonEnabled = value;
            modelServerButton.ButtonEnabled = value;
            gamificationServerButton.ButtonEnabled = value;
            sharingServerButton.ButtonEnabled = value;
            collisionButton.ButtonEnabled = value;
        }
    }

    // Use this for initialization
    void Start()
    {
        InitializeButtons();
    }

    private void InitializeButtons()
    {
        // get/create buttons
        closeButton = transform.Find("Close Button").gameObject.AddComponent<FocusableButton>();
        languageButton = transform.Find("Language Button").gameObject.AddComponent<FocusableContentButton>();
        modelServerButton = transform.Find("Model Server Button").gameObject.AddComponent<FocusableContentButton>();
        gamificationServerButton = transform.Find("Gamification Server Button").gameObject.AddComponent<FocusableContentButton>();
        sharingServerButton = transform.Find("Sharing Server Button").gameObject.AddComponent<FocusableContentButton>();
        collisionButton = transform.Find("Collision Detection Button").gameObject.AddComponent<FocusableContentButton>();

        SettingsActions actions = gameObject.AddComponent<SettingsActions>();

        // set button actions
        closeButton.OnPressed = Close;
        languageButton.OnPressed = ChangeLanguage;
        modelServerButton.OnPressed = actions.EnterServer;
        gamificationServerButton.OnPressed = actions.EnterGamificationServer;
        sharingServerButton.OnPressed = actions.EnterSharingServer;
        collisionButton.OnPressed = () =>
        {
            actions.ToggleCollision();
            SetButtonContents();
        };

        // set captions
        closeButton.Text = LocalizationManager.Instance.ResolveString("Close");
        languageButton.Text = LocalizationManager.Instance.ResolveString("Language");
        modelServerButton.Text = LocalizationManager.Instance.ResolveString("Model Server");
        gamificationServerButton.Text = LocalizationManager.Instance.ResolveString("Gamification Server");
        sharingServerButton.Text = LocalizationManager.Instance.ResolveString("Sharing Server");
        collisionButton.Text = LocalizationManager.Instance.ResolveString("Collision Detection");

        // set contents
        SetButtonContents();
    }

    private void ChangeLanguage()
    {
        GameObject languageInstance = Instantiate(languageMenu);
        LanguageMenu languageScript = languageInstance.GetComponent<LanguageMenu>();
        languageScript.OnCloseAction = () =>
        {
            SetButtonContents();
            MenuEnabled = true;
        };
        languageInstance.transform.parent = transform;
        languageInstance.transform.rotation = transform.rotation;
        languageInstance.transform.localPosition = Vector3.zero;
        languageInstance.transform.Translate(-0.1f, 0.01f, 0.01f);
        MenuEnabled = false;
    }

    private void OnEnable()
    {
        // set contents
        SetButtonContents();
    }

    private void SetButtonContents()
    {
        if (languageButton != null)
        {
            languageButton.Content = LocalizationManager.Instance.ResolveString(InformationManager.Instance.Language.ToString());
            modelServerButton.Content = InformationManager.Instance.BackendServer;
            gamificationServerButton.Content = InformationManager.Instance.GamificationServer;
            sharingServerButton.Content = InformationManager.Instance.SharingServer;
            collisionButton.Content = InformationManager.Instance.CollisionEnabled ?
                LocalizationManager.Instance.ResolveString("On") : LocalizationManager.Instance.ResolveString("Off");
        }
    }

    private void Close()
    {
        if (OnCloseAction != null)
        {
            OnCloseAction();
        }
        Destroy(gameObject);
    }
}
