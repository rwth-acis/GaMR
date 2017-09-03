using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageMenu : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        InitializeButtons();
    }

    private void InitializeButtons()
    {
        // get/create buttons
        FocusableContentButton englishButton = transform.Find("English Button").gameObject.AddComponent<FocusableContentButton>();
        FocusableContentButton germanButton = transform.Find("German Button").gameObject.AddComponent<FocusableContentButton>();
        FocusableContentButton dutchButton = transform.Find("Dutch Button").gameObject.AddComponent<FocusableContentButton>();

        SettingsActions actions = gameObject.AddComponent<SettingsActions>();

        // set button data
        englishButton.Data = (int) Language.ENGLISH;
        germanButton.Data = (int) Language.GERMAN;
        dutchButton.Data = (int)Language.DUTCH;

        // set button actions
        englishButton.OnButtonPressed = ChangeLanguage;
        germanButton.OnButtonPressed = ChangeLanguage;
        dutchButton.OnButtonPressed = ChangeLanguage;

        // set captions
        englishButton.Text = LocalizationManager.Instance.ResolveString("English");
        germanButton.Text = LocalizationManager.Instance.ResolveString("German");
        dutchButton.Text = LocalizationManager.Instance.ResolveString("Dutch");
    }

    private void ChangeLanguage(Button sender)
    {
        InformationManager.Instance.Language = (Language)sender.Data;
        Destroy(gameObject);
    }
}
