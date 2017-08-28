﻿using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using HoloToolkit.Unity.SpatialMapping;

/// <summary>
/// Manages important settings of the application
/// contains definitions for the IP address of the backend,
/// the port and a property which combines all into a http address which can be called
/// also provides information about the user
/// </summary>
public class InformationManager : Singleton<InformationManager>
{

    [SerializeField]
    private string ipAddressBackend = "192.168.178.43";
    [SerializeField]
    private string ipAddressGamification = "192.168.178.75";
    public int portBackend = 8080;
    public int portGamification = 8086;
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
    public string FullBackendAddress { get { return "http://" + ipAddressBackend + ":" + portBackend.ToString(); } }
    public string IPAddressBackend { get { return ipAddressBackend; } set { ipAddressBackend = value; ipAddressGamification = value; } }
    public string IPAddressGamification { get { return "http://" + ipAddressGamification; } }
    public string GamificationAddress { get { return IPAddressGamification + ":" + portGamification; } }

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
        PlayerPrefs.SetString("ipAddress", ipAddressBackend);
        PlayerPrefs.SetInt("port", portBackend);
        PlayerPrefs.SetInt("language", (int)language);
        PlayerPrefs.Save();
        Debug.Log("Data saved");
    }

    private void LoadValues()
    {
        IPAddressBackend = PlayerPrefs.GetString("ipAddress", "192.0.0.0");
        portBackend = PlayerPrefs.GetInt("port", 8080);
        this.Language = (Language)PlayerPrefs.GetInt("language", 0);
        Debug.Log("Loaded " + ipAddressBackend + ":" + portBackend);
        Debug.Log("Language: " + language);
    }
}

public enum PlayerType
{
    ALL, AUTHOR, STUDENT
}
