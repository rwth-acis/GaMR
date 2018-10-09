using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public void Login()
    {
        AuthorizationManager.Instance.Login();
    }

    public void ShowSettings()
    {
        Debug.Log("Show settings");
    }

}
