using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using HoloToolkit.Unity.SpatialMapping;
using HoloToolkit.Sharing;

/// <summary>
/// Manages important settings of the application
/// contains definitions for the IP address of the backend,
/// the port and a property which combines all into a http address which can be called
/// also provides information about the user
/// </summary>
public class InformationManager : Singleton<InformationManager>
{

    [SerializeField]
    private string backendServer = "192.168.178.43";
    [SerializeField]
    private string gamificationServer = "192.168.178.75";
    [SerializeField]
    private string sharingServer = "192.168.178.48";
    private bool sharingEnabled = true;
    [SerializeField]
    private int portBackend = 8080;
    [SerializeField]
    private int portGamification = 8086;
    public PlayerType playerType = PlayerType.STUDENT;
    [SerializeField]
    private Language language = Language.ENGLISH;

    private bool collisionEnabled = true;

    public void Start()
    {
        LoadValues();
    }

    /// <summary>
    /// http address which combines the ip address and the port
    /// </summary>
    public string FullBackendAddress { get { return "http://" + backendServer + ":" + portBackend.ToString(); } }
    public string BackendServer { get { return backendServer; } set { backendServer = value; } }
    public string GamificationServer { get { return gamificationServer; } set { gamificationServer = value; } }
    public string FullGamificationAddress { get { return "http://" + gamificationServer + ":" + portGamification; } }
    public string SharingServer
    {
        get { return sharingServer; }
        set
        {
            sharingServer = value;
            if (SharingStage.Instance != null)
            {
                SharingStage.Instance.ServerAddress = sharingServer;
                Debug.Log("Sharing ip set to " + sharingServer);
                SharingStage.Instance.ConnectToServer();
            }
        }
    }

    public bool SharingEnabled
    {
        get { return sharingEnabled; }
        set
        {
            sharingEnabled = value;
            if (SharingStage.Instance != null)
            {
                SharingStage.Instance.gameObject.SetActive(value);
                if (value)
                {
                    SharingStage.Instance.ConnectToServer();
                }
            }
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        SharingServer = PlayerPrefs.GetString("sharingServer");
        SharingEnabled = (1 == PlayerPrefs.GetInt("sharingEnabled", 1));
    }

    public bool CollisionEnabled
    {
        get { return collisionEnabled; }
        set
        {
            collisionEnabled = value;
            SpatialMappingManager.Instance.gameObject.SetActive(value);
        }
    }

    public UserInfo UserInfo { get; set; }

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

    private void SaveValues()
    {
        PlayerPrefs.SetString("backendServer", backendServer);
        PlayerPrefs.SetString("gamificationServer", gamificationServer);
        PlayerPrefs.SetString("sharingServer", SharingServer);
        PlayerPrefs.SetInt("language", (int)language);
        if (SharingEnabled)
        {
            PlayerPrefs.SetInt("sharingEnabled", 1);
        }
        else
        {
            PlayerPrefs.SetInt("sharingEnabled", 0);
        }
        if (collisionEnabled)
        {
            PlayerPrefs.SetInt("collisionEnabled", 1);
        }
        else
        {
            PlayerPrefs.SetInt("collisionEnabled", 0);
        }
        PlayerPrefs.Save();
        Debug.Log("Data saved");
    }

    private void LoadValues()
    {
        BackendServer = PlayerPrefs.GetString("backendServer", "192.168.178.82");
        GamificationServer = PlayerPrefs.GetString("gamificationServer", "192.168.178.82");
        SharingServer = PlayerPrefs.GetString("sharingServer", "192.168.178.82");
        this.Language = (Language)PlayerPrefs.GetInt("language", 0);
        SharingEnabled = (1 == PlayerPrefs.GetInt("sharingEnabled", 1));
        CollisionEnabled = (1 == PlayerPrefs.GetInt("collisionEnabled", 1));
        Debug.Log("Loaded " + backendServer + ":" + portBackend);
        Debug.Log("Language: " + language);
    }
}

public enum PlayerType
{
    ALL, AUTHOR, STUDENT
}
