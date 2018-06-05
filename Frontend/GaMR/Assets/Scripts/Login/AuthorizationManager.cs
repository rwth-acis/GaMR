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
    private string learningLayersClientId = "c4ced10f-ce0f-4155-b6f7-a4c40ffa410c";
    [SerializeField]
    private string googleClientId = "173179682466-gm9uiqbo2717nfiuqhq13mi1s23u1pob.apps.googleusercontent.com";
    [SerializeField]
    private string debugToken;
    private string accessToken;
    private string refreshToken;

    private string authorizationCode;

    private AuthorizationProvider selectedAuthorizationProvider;

    private void Start()
    {
        // skip the login by using the debug token
        if (Application.isEditor && (accessToken == null || accessToken == ""))
        {
            accessToken = debugToken;
            selectedAuthorizationProvider = AuthorizationProvider.LEARNING_LAYERS;
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
        else if (req.responseCode == 401)
        {
            Debug.LogError("Unauthorized: access token is wrong");
        }
    }

    public void Login(AuthorizationProvider provider)
    {
        selectedAuthorizationProvider = provider;
        if (Application.isEditor)
        {
            SceneManager.LoadScene("Scene", LoadSceneMode.Single);
            return;
        }
        if (provider == AuthorizationProvider.LEARNING_LAYERS)
        {
            Application.OpenURL("https://api.learning-layers.eu/o/oauth2/authorize?response_type=token&scope=openid%20profile%20email&client_id=" + learningLayersClientId + "&redirect_uri=gamr://");
        }
        else if  (provider == AuthorizationProvider.GOOGLE)
        {
            Application.OpenURL("https://accounts.google.com/o/oauth2/v2/auth?response_type=code&scope=openid%20email&redirect_uri=com.gamr%3A%2Foauth2redirect&client_id=" + googleClientId + "&hd=");
        }
    }

    public void Logout()
    {
        accessToken = "";
        authorizationCode = "";
        refreshToken = "";
        SceneManager.LoadScene("Login", LoadSceneMode.Single);
    }

    private void StartedByProtocol(Uri uri)
    {
        if (selectedAuthorizationProvider == AuthorizationProvider.LEARNING_LAYERS)
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
        }
        else if (selectedAuthorizationProvider == AuthorizationProvider.GOOGLE)
        {
            if (uri.Fragment != null)
            {
                char[] splitters = { '?', '&' };
                string[] arguments = uri.AbsoluteUri.Split(splitters);
                foreach (string argument in arguments)
                {
                    if (argument.StartsWith("code="))
                    {
                        authorizationCode = argument.Replace("code=", "");
                        Debug.Log("authorization code: " + authorizationCode);
                        RestManager.Instance.POST("https://www.googleapis.com/oauth2/v4/token?code=" + authorizationCode + "&client_id=" + googleClientId
                            + "&redirect_uri=com.gamr%3A%2Foauth2redirect&grant_type=authorization_code", (req) =>
                            {
                                Debug.Log(req.downloadHandler.text);
                                if (req.responseCode != 200)
                                {
                                    MessageBox.Show(LocalizationManager.Instance.ResolveString("Could not exchange authorization token for access token"), MessageBoxType.ERROR);
                                    return;
                                }
                                // else:
                                string json = req.downloadHandler.text;
                                Debug.Log(json);
                            });
                    }
                    else if (argument.StartsWith("error="))
                    {
                        MessageBox.Show(LocalizationManager.Instance.ResolveString("Login failed") + ": " + argument.Replace("error=", ""), MessageBoxType.ERROR);
                        return;
                    }
                }
            }
        }



        Debug.Log("The access token is " + accessToken);

        if (accessToken != "")
        {
            AddAccessTokenToHeader();
            CheckAccessToken();
        }
        else
        {
            MessageBox.Show("User could not be logged in", MessageBoxType.ERROR);
        }
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
        RestManager.Instance.GET("https://api.learning-layers.eu/o/oauth2/userinfo?access_token=" + accessToken, OnLogin);
    }

    private void OnLogin(UnityWebRequest result)
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
            MessageBox.Show(LocalizationManager.Instance.ResolveString("An error concerning the user data occured. The login failed.\nCode: ") + req.responseCode + "\n" + req.downloadHandler.text, MessageBoxType.ERROR);
        }
    }
}

public enum AuthorizationProvider
{
    LEARNING_LAYERS, GOOGLE
}
