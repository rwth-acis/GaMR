using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreetUser : MonoBehaviour
{

    /// <summary>
    /// Greets the user on startup
    /// </summary>
    void Start()
    {
        if (InformationManager.Instance.UserInfo != null)
        {
            MessageBox.Show(LocalizationManager.Instance.ResolveString("Hello") + ", " + InformationManager.Instance.UserInfo.name, MessageBoxType.INFORMATION);
        }
    }
}
