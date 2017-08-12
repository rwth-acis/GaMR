using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginActions : MonoBehaviour
{

    public void ShowLoginForm()
    {
        //GameObject form = (GameObject)Instantiate(Resources.Load("LoginForm"));
        Application.OpenURL("https://api.learning-layers.eu/o/oauth2/authorize?response_type=token&scope=openid%20profile%20email&client_id=c4ced10f-ce0f-4155-b6f7-a4c40ffa410c&redirect_uri=gamr://");


    }
}
