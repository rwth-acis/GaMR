using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuActions : MonoBehaviour
{

    Menu menu;
    InformationManager infoManager;

    public InformationManager InfoManager { get { return infoManager; } }

    public void Start()
    {
        GameObject gameInfo = GameObject.Find("InformationManager");
        if (gameInfo != null)
        {
            infoManager = gameInfo.GetComponent<InformationManager>();
        }
    }

    public void EnterIPAddress()
    {
        Keyboard.Display("Enter the IP-Address", SetIPAddress);
        gameObject.SetActive(false);
    }

    public void SetIPAddress(string address)
    {
        gameObject.SetActive(true);
        // if not null => input was accepted by user
        if (address != null)
        {
            Debug.Log("Set IP Address to " + address);
            infoManager.ipAddressBackend = address;
        }
    }
}
