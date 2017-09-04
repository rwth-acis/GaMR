using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SettingsActions : MonoBehaviour {

    /// <summary>
    /// Displays a keyboard in order to enter the ip address
    /// </summary>
    public void EnterServer()
    {
        Keyboard.Display(LocalizationManager.Instance.ResolveString("Enter the Server"), InformationManager.Instance.BackendServer, SetServer, true);
        gameObject.SetActive(false);
    }

    public void EnterGamificationServer()
    {
        Keyboard.Display(LocalizationManager.Instance.ResolveString("Enter Gamification Server"), InformationManager.Instance.GamificationServer, SetGamificationServer, true);
        gameObject.SetActive(false);
    }

    public void EnterSharingServer()
    {
        Keyboard.Display(LocalizationManager.Instance.ResolveString("Enter Sharing Server"), InformationManager.Instance.SharingServer, SetSharingServer, true);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Sets the ip address which was entered before
    /// called by the keyboard which was created in EnterIPAddress
    /// </summary>
    /// <param name="address">The address which has been typed by the user (null if cancelled)</param>
    public void SetServer(string address)
    {
        // if not null => input was accepted by user
        if (address != null)
        {
            Debug.Log("Set Server to " + address);
            InformationManager.Instance.BackendServer = address;
            TestServer();
        }
        gameObject.SetActive(true);
    }

    public void SetGamificationServer(string address)
    {
        // if not null => input was accepted by user
        if (address != null)
        {
            Debug.Log("Set Gamification Server to " + address);
            InformationManager.Instance.GamificationServer = address;
        }
        gameObject.SetActive(true);
    }

    private void SetSharingServer(string address)
    {
        // if not null => input was accepted by user
        if (address != null)
        {
            Debug.Log("Set Sharing Server to " + address);
            InformationManager.Instance.SharingServer = address;
        }
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Test if the server is responding by requesting the model overview
    /// </summary>
    private void TestServer()
    {
        WaitCursor.Show();
        RestManager.Instance.GET(InformationManager.Instance.FullBackendAddress + "/resources/model/overview", RestResult, null);
    }

    internal void ToggleSharing()
    {
        InformationManager.Instance.SharingEnabled = !InformationManager.Instance.SharingEnabled;
    }

    /// <summary>
    /// Processes the result of the TestAddress web request
    /// </summary>
    /// <param name="result">The result of the request</param>
    private void RestResult(UnityWebRequest result, object[] args)
    {
        WaitCursor.Hide();
        if (result.responseCode == 200)
        {
            MessageBox.Show(LocalizationManager.Instance.ResolveString("Address successfully saved") + Environment.NewLine +
                LocalizationManager.Instance.ResolveString("The server is responding"), MessageBoxType.SUCCESS);
        }
        else
        {
            MessageBox.Show(LocalizationManager.Instance.ResolveString("Address successfully saved") + Environment.NewLine +
                LocalizationManager.Instance.ResolveString("However, the server does not respond"), MessageBoxType.WARNING);
        }
    }

    /// <summary>
    /// Sets the language to the specified value
    /// </summary>
    /// <param name="language">The new language</param>
    private void SetLanguage(Language language)
    {
        InformationManager.Instance.Language = language;
    }

    public void ToggleCollision()
    {
        InformationManager.Instance.CollisionEnabled = !InformationManager.Instance.CollisionEnabled;
    }
}
