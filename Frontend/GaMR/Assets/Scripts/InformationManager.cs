using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages important settings of the application
/// contains definitions for the IP address of the backend,
/// the port and a property which combines all into a http address which can be called
/// also provides information about the user
/// </summary>
public class InformationManager : MonoBehaviour {

    public static InformationManager instance;

    public string ipAddressBackend = "192.168.178.43";
    public int portBackend = 8080;
    public PlayerType playerType = PlayerType.STUDENT;
    [SerializeField]
    private Language language = Language.ENGLISH;

    public void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    /// <summary>
    /// http address which combines the ip address and the port
    /// </summary>
    public string BackendAddress { get { return "http://" + ipAddressBackend + ":" + portBackend.ToString(); } }

    public Language Language
    {
        get { return language; }
        set { language = value; }
    }
}

public enum PlayerType
{
    ALL, AUTHOR, STUDENT
}
