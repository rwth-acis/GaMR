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

    public void EnterPort()
    {
        Keyboard.Display("Enter the Port", SetIPAddress);
        gameObject.SetActive(false);
    }

    public void SetIPPort(string port)
    {
        gameObject.SetActive(true);
        // if not null => input was accepted by user
        if (port != null)
        {
            int iPort;
            if (int.TryParse(port, out iPort))
            {
                Debug.Log("Set Port to " + port);
                infoManager.portBackend = iPort;
            }
            else
            {
                MessageBox.Show("Input was not a number" + Environment.NewLine + "Could not set port", MessageBoxType.ERROR);
            }
        }
    }
}
