using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class AuthorizationManager : Singleton<AuthorizationManager>
{

    [SerializeField]
    private string debugToken;
    private string accessToken;

    private void Start()
    {
        // skip the login by using the debug token
        if (Application.isEditor && (accessToken == null || accessToken == ""))
        {
            accessToken = debugToken;
            AddAccessTokenToHeader();
            RestManager.Instance.GET("https://api.learning-layers.eu/o/oauth2/userinfo?access_token=" + accessToken, GetUserInfoForDebugToken);
        }
    }

    private void GetUserInfoForDebugToken(UnityWebRequest req)
    {
        if (req.responseCode == 200)
        {
            string json = req.downloadHandler.text;
            Debug.Log(json);
            UserInfo info = JsonUtility.FromJson<UserInfo>(json);
            InformationManager.Instance.UserInfo = info;
        }
    }

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

        AddAccessTokenToHeader();
        CheckAccessToken();
    }

    private void AddAccessTokenToHeader()
    {
        if (RestManager.Instance.StandardHeader.ContainsKey("access_token"))
        {
            RestManager.Instance.StandardHeader["access_token"] = accessToken;
        }
        else
        {
            RestManager.Instance.StandardHeader.Add("access_token", accessToken);
        }
    }

    private void CheckAccessToken()
    {
        RestManager.Instance.GET("https://api.learning-layers.eu/o/oauth2/userinfo?access_token=" + accessToken, Login, null);
    }

    private void Login(UnityWebRequest result, object[] args)
    {
        if (result.responseCode == 200)
        {
            string json = result.downloadHandler.text;
            Debug.Log(json);
            UserInfo info = JsonUtility.FromJson<UserInfo>(json);
            InformationManager.Instance.UserInfo = info;
            GamificationFramework.Instance.ValidateLogin(LoginValidated);
        }
        else
        {
            MessageBox.Show(LocalizationManager.Instance.ResolveString("Error while retrieving the user data. Login failed"), MessageBoxType.ERROR);
        }
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
