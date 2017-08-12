using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AuthorizationManager : Singleton<AuthorizationManager>
{

    private string accessToken;

    //public static void Authorize(string name, string password)
    //   {
    //       if (name == "Student")
    //       {
    //           InformationManager.Instance.playerType = PlayerType.STUDENT;
    //       }
    //       else
    //       {
    //           InformationManager.Instance.playerType = PlayerType.AUTHOR;
    //       }
    //       SceneManager.LoadScene("Scene", LoadSceneMode.Single);
    //   }

    private void StartedByProtocol(Uri uri)
    {
        if (uri.Fragment != null)
        {
            char[] splitters = { '#', '&' };
            string[] arguments = uri.Fragment.Split(splitters);
            foreach (string argument in arguments)
            {
                if (argument.StartsWith("access_token="))
                {
                    accessToken = argument.Replace("access_token=", "");
                    break;
                }
            }

        }

        Debug.Log("The access token is " + accessToken);

        if (CheckAccessToken())
        {
            SceneManager.LoadScene("Scene", LoadSceneMode.Single);
        }
    }

    private bool CheckAccessToken()
    {
        return true;
    }


}
