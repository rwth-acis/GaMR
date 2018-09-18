using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Net.Sockets;
using System.Net;
using System.Threading;

public class AuthorizationManager : Singleton<AuthorizationManager>
{

    [SerializeField]
    private string clientId = "c4ced10f-ce0f-4155-b6f7-a4c40ffa410c";
    private string clientSecret;
    [SerializeField]
    private string debugToken;
    private string accessToken;
    private string authorizationCode;

    const string learningLayersAuthorizationEndpoint = "https://api.learning-layers.eu/o/oauth2/authorize";
    const string learningLayersTokenEndpoint = "https://api.learning-layers.eu/o/oauth2/token";
    const string learningLayersUserInfoEndpoint = "https://api.learning-layers.eu/o/oauth2/userinfo";

    const string scopes = "openid%20profile%20email";

    string gamrRedirectURI = "http://127.0.0.1";

    private Thread serverThread;
    private bool serverActive = false;
    private HttpListener http;
    private bool loginFromServerSuccessful = false;

    public string AccessToken { get { return accessToken; } }

    private void Start()
    {
        // skip the login by using the debug token
        if (Application.isEditor)
        {
            if (accessToken == null || accessToken == "")
            {
                accessToken = debugToken;
                AddAccessTokenToHeader();
                RestManager.Instance.GET(learningLayersUserInfoEndpoint + "?access_token=" + accessToken, GetUserInfoForDebugToken);
            }
        }
        else // else: fetch the client secret
        {
            TextAsset secretAsset = (TextAsset)Resources.Load("values/client_secret");
            clientSecret = secretAsset.text;
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
        //if (Application.isEditor)
        //{
        //    SceneManager.LoadScene("Scene", LoadSceneMode.Single);
        //    return;
        //}

        if (string.IsNullOrEmpty(gamrRedirectURI))
        {
            gamrRedirectURI = GenerateRedirectUri();
        }
        StartServer();
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
                        "&client_secret=" + clientSecret + "&redirect_uri=" + gamrRedirectURI + "&grant_type=authorization_code", (req) =>
                        {
                            string json = req.downloadHandler.text;
                            AuthorizationFlowAnswer answer = JsonUtility.FromJson<AuthorizationFlowAnswer>(json);
                            if (!string.IsNullOrEmpty(answer.error))
                            {
                                MessageBox.Show(answer.error_description, MessageBoxType.ERROR);
                            }
                            else
                            {
                                // extract access token and check it
                                accessToken = answer.access_token;

                                Debug.Log("The access token is " + accessToken);

                                AddAccessTokenToHeader();
                                CheckAccessToken();
                            }
                        }
                        );
                    break;
                }
            }

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

    // ----------- VR changes
    private void StartServer()
    {
        serverThread = new Thread(Listen);
        serverActive = true;
        serverThread.Start();
    }

    private void StopServerImmediately()
    {
        serverThread.Abort();
        http.Stop();
        Debug.Log("HTTPListener stopped.");
    }

    private void Listen()
    {
        http = new HttpListener();
        if (string.IsNullOrEmpty(gamrRedirectURI))
        {
            gamrRedirectURI = GenerateRedirectUri();
        }
        http.Prefixes.Add(gamrRedirectURI);
        http.Start();
        Debug.Log("HTTPListener listening...");

        while (serverActive)
        {
            try
            {
                HttpListenerContext context = http.GetContext();

                if (context.Request.QueryString.Get("error") != null)
                {
                    Debug.Log("Error: " + context.Request.QueryString.Get("error"));
                    return;
                }
                if (context.Request.QueryString.Get("access_token") == null)
                {
                    Debug.Log("Login response did not contain an access token");
                    return;
                }

                accessToken = context.Request.QueryString.Get("access_token");

                string responseString = string.Format("<html><head></head><body>" + LocalizationManager.Instance.ResolveString("Please return to the app") + "</body></html>");
                var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                context.Response.ContentLength64 = buffer.Length;
                var responseOutput = context.Response.OutputStream;
                responseOutput.Write(buffer, 0, buffer.Length);
                responseOutput.Close();
                http.Stop();
                serverActive = false;
                Debug.Log("Server stopped");

                //AddAccessTokenToHeader();
                //CheckAccessToken();

                loginFromServerSuccessful = true; // we need this since loading a scene can only be done on the main thread
            }
            catch(Exception e)
            {
                Debug.Log("Error while listening: " + e);
            }
        }
    }

    private void Update()
    {
        // checks if the server thread processed a login response and now wants to change the scene
        if (loginFromServerSuccessful)
        {
            loginFromServerSuccessful = false;
            AddAccessTokenToHeader();
            CheckAccessToken();
        }
    }

    private int GetUnusedPort()
    {
        TcpListener listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    private string GenerateRedirectUri()
    {
        string red_uri = "http://" + IPAddress.Loopback + ":" + GetUnusedPort() + "/";
        Debug.Log("redirect uri is " + red_uri);
        return red_uri;
    }

    private void OnApplicationQuit()
    {
        StopServerImmediately();
    }
}
