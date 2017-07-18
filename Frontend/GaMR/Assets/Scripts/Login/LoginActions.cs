using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginActions : MonoBehaviour {

	public void ShowLoginForm()
    {
        GameObject form = (GameObject)Instantiate(Resources.Load("LoginForm"));
    }
}
