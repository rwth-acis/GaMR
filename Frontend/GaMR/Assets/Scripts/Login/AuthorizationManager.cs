using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AuthorizationManager : Singleton<AuthorizationManager> {

	public static void Authorize(string name, string password)
    {
        if (name == "Student")
        {
            InformationManager.Instance.playerType = PlayerType.STUDENT;
        }
        else
        {
            InformationManager.Instance.playerType = PlayerType.AUTHOR;
        }
        SceneManager.LoadScene("Scene", LoadSceneMode.Single);
    }
}
