using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class AuthorizationManager : Singleton<AuthorizationManager>
{

    private string accessToken;

    public void Logout()
    {
        accessToken = "";
    }

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

        CheckAccessToken();
    }

    private void CheckAccessToken()
    {
        if (RestManager.Instance.StandardHeader.ContainsKey("access_token"))
        {
            RestManager.Instance.StandardHeader["access_token"] = accessToken;
        }
        else
        {
            RestManager.Instance.StandardHeader.Add("access_token", accessToken);
        }

        RestManager.Instance.GET("https://api.learning-layers.eu/o/oauth2/userinfo?access_token=" + accessToken, GetUserInfo);
    }

    private void GetUserInfo(string json)
    {
        Debug.Log(json);
        UserInfo info = JsonUtility.FromJson<UserInfo>(json);
        InformationManager.Instance.UserInfo = info;
        GamificationFramework.Instance.ValidateLogin(LoginValidated);
    }

    private void LoginValidated(UnityWebRequest req)
    {
        if (req.responseCode == 200)
        {
            SceneManager.LoadScene("Scene", LoadSceneMode.Single);
        }
        else if (req.responseCode == 401)
        {
            MessageBox.Show(LocalizationManager.Instance.ResolveString("The login could not be validated"), MessageBoxType.ERROR);
        }
        else
        {
            MessageBox.Show(LocalizationManager.Instance.ResolveString("An error concerning the user data occured. The login failed."), MessageBoxType.ERROR);
        }
    }
}
