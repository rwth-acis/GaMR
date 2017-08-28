using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HoloToolkit.Sharing;

/// <summary>
/// Manages important settings of the application
/// contains definitions for the IP address of the backend,
/// the port and a property which combines all into a http address which can be called
/// also provides information about the user
/// </summary>
public class InformationManager : Singleton<InformationManager> {

    public string ipAddressBackend = "192.168.178.48";
    [SerializeField]
    private string sharingIpAddress = "192.168.178.48";
    public int portBackend = 8080;
    public PlayerType playerType = PlayerType.STUDENT;
    [SerializeField]
    private Language language = Language.ENGLISH;

    public void Start()
    {
        LoadValues();
    }

    /// <summary>
    /// http address which combines the ip address and the port
    /// </summary>
    public string BackendAddress { get { return "http://" + ipAddressBackend + ":" + portBackend.ToString(); } }

    public string SharingBackendAddress { get { return sharingIpAddress; } set {
            sharingIpAddress = value;
            if (SharingStage.Instance != null)
            {
                SharingStage.Instance.ServerAddress = sharingIpAddress;
            }
        }
    }

    public Language Language
    {
        get { return language; }
        set { language = value; LocalizationManager.Instance.UpdateLanguage(); }
    }

    protected override void OnDestroy()
    {
        SaveValues();
    }

    public void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            SaveValues();
        }
    }

    public void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveValues();
        }
    }

    public void OnLevelWasLoaded(int level)
    {
        if (SharingStage.Instance != null)
        {
            SharingStage.Instance.ServerAddress = SharingBackendAddress;
        }
    }

    private void SaveValues()
    {
        PlayerPrefs.SetString("ipAddress", ipAddressBackend);
        PlayerPrefs.SetInt("port", portBackend);
        PlayerPrefs.SetString("sharingAddress", sharingIpAddress);
        PlayerPrefs.SetInt("language", (int)language);
        PlayerPrefs.Save();
        Debug.Log("Data saved");
    }

    private void LoadValues()
    {
        ipAddressBackend = PlayerPrefs.GetString("ipAddress", "192.0.0.0");
        SharingBackendAddress = PlayerPrefs.GetString("sharingAddress", "192.0.0.0");
        portBackend = PlayerPrefs.GetInt("port", 8080);
        this.Language = (Language) PlayerPrefs.GetInt("language", 0);
        Debug.Log("Loaded " + ipAddressBackend + ":" + portBackend);
        Debug.Log("Language: " + language);
    }
}

public enum PlayerType
{
    ALL, AUTHOR, STUDENT
}
