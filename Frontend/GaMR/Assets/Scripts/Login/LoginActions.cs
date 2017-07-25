using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginActions : MonoBehaviour {

	public void ShowLoginForm()
    {
        GameObject form = (GameObject)Instantiate(Resources.Load("LoginForm"));
        //Application.OpenURL("https://api.learning-layers.eu/o/oauth2/login");
        
    }
}
