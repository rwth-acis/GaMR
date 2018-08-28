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
    private string clientId = "c4ced10f-ce0f-4155-b6f7-a4c40ffa410c";
    [SerializeField]
    private string debugToken;
    private string accessToken;
    private string authorizationCode;

    const string learningLayersAuthorizationEndpoint = "https://api.learning-layers.eu/o/oauth2/authorize";
    const string learningLayersTokenEndpoint = "https://api.learning-layers.eu/o/oauth2/token";
    const string learningLayersUserInfoEndpoint = "https://api.learning-layers.eu/o/oauth2/userinfo";

    const string scopes = "openid%20profile%20email";

    const string gamrRedirectURI = "gamr://";

    public string AccessToken { get { return accessToken; } }

    private void Start()
    {
        // skip the login by using the debug token
        if (Application.isEditor && (accessToken == null || accessToken == ""))
        {
            accessToken = debugToken;
            AddAccessTokenToHeader();
            RestManager.Instance.GET(learningLayersUserInfoEndpoint + "?access_token=" + accessToken, GetUserInfoForDebugToken);
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

    public void Login()
    {
        if (Application.isEditor)
        {
            SceneManager.LoadScene("Scene", LoadSceneMode.Single);
            return;
        }
        Application.OpenURL(learningLayersAuthorizationEndpoint + "?response_type=code&scope=" + scopes + "&client_id=" + clientId + "&redirect_uri=" + gamrRedirectURI);
    }

    public void Logout()
    {
        accessToken = "";
        SceneManager.LoadScene("Login", LoadSceneMode.Single);
    }

    private void StartedByProtocol(Uri uri)
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
                    Debug.Log("authorizationCode: " + authorizationCode);
                    // now exchange authorization code for access token
                    RestManager.Instance.POST(learningLayersTokenEndpoint + "?code=" + authorizationCode + "&client_id=" + clientId +
                        "&redirect_uri=" + gamrRedirectURI + "&grant_type=authorization_code", (req) => // TODO: add client secret
                        {
                            string json = req.downloadHandler.text;
                        }
                        );
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
        RestManager.Instance.GET(learningLayersUserInfoEndpoint + "?access_token=" + accessToken, OnLogin);
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
