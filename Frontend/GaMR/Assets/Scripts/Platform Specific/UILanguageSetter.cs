using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILanguageSetter : MonoBehaviour, ILanguageUpdater
{
    Text loginText;
    Text settingsText;
    Text languageText;
    Text serverAddressesText;


    // Use this for initialization
    private void Start()
    {
        LocalizationManager.Instance.AddUpdateReceiver(this);
        FindButtonCaptions();
    }

    private void FindButtonCaptions()
    {
        loginText = transform.Find("Overall Background/Login Menu Buttons/Button Login/Text").GetComponent<Text>();
        settingsText = transform.Find("Overall Background/Login Menu Buttons/Button Settings/Text").GetComponent<Text>();
        languageText = transform.Find("Overall Background/Settings Buttons/Button Language/Text").GetComponent<Text>();
        serverAddressesText = transform.Find("Overall Background/Settings Buttons/Button Server Addresses/Text").GetComponent<Text>();

        OnUpdateLanguage();
    }

    private void OnDestroy()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.RemoveUpdateReceiver(this);
        }
    }

    public void OnUpdateLanguage()
    {
        loginText.text = LocalizationManager.Instance.ResolveString("Login");
        settingsText.text = LocalizationManager.Instance.ResolveString("Settings");
        languageText.text = LocalizationManager.Instance.ResolveString("Language");
        serverAddressesText.text = LocalizationManager.Instance.ResolveString("Server Addresses");
    }
}
