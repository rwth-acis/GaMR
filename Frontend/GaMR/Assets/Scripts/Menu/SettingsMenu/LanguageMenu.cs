using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageMenu : BaseMenu
{
    FocusableContentButton englishButton;
    FocusableContentButton germanButton;
    FocusableContentButton dutchButton;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        InitializeButtons();
    }

    public Action OnCloseAction { get; set; }

    private void InitializeButtons()
    {
        // get/create buttons
        englishButton = transform.Find("English Button").gameObject.GetComponent<FocusableContentButton>();
        germanButton = transform.Find("German Button").gameObject.GetComponent<FocusableContentButton>();
        dutchButton = transform.Find("Dutch Button").gameObject.GetComponent<FocusableContentButton>();

        SettingsActions actions = gameObject.AddComponent<SettingsActions>();

        // set button data
        englishButton.Data = (int) Language.ENGLISH;
        germanButton.Data = (int) Language.GERMAN;
        dutchButton.Data = (int)Language.DUTCH;

        OnUpdateLanguage();

        // set button actions
        englishButton.OnButtonPressed = ChangeLanguage;
        germanButton.OnButtonPressed = ChangeLanguage;
        dutchButton.OnButtonPressed = ChangeLanguage;
    }

    private void ChangeLanguage(GaMRButton sender)
    {
        InformationManager.Instance.Language = (Language)sender.Data;
        if (OnCloseAction != null)
        {
            OnCloseAction();
        }
        Destroy(gameObject);
    }

    public override void OnUpdateLanguage()
    {
        // set captions
        englishButton.Text = LocalizationManager.Instance.ResolveString("English");
        germanButton.Text = LocalizationManager.Instance.ResolveString("German");
        dutchButton.Text = LocalizationManager.Instance.ResolveString("Dutch");
    }
}
