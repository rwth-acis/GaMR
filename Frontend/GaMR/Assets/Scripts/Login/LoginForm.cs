using System;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using HoloToolkit.Unity;

public class LoginForm : MonoBehaviour, IWindow
{
    [Tooltip("The gameobject with the textmesh that displays the user name")]
    public GameObject userNameInputText;
    [Tooltip("The background of the user name input field")]
    public GameObject userNameInputField;
    [Tooltip("The gameobject with the textmesh that displays the password")]
    public GameObject passwordInputText;
    [Tooltip("The background of the password input field")]
    public GameObject passwordInputField;
    [Tooltip("The button-gameobject")]
    public GameObject confirmButtonObject;

    private TextMesh userNameCaption;
    private TextMesh passwordCaption;

    private string userName = "";
    private string password = "";

    private SimpleTagalong tagalongScript;

    private string UserName
    {
        get { return userName; }
        set { userName = value; userNameCaption.text = value; }
    }

    private string Password
    {
        get { return password; }
        set { password = value;
            passwordCaption.text = DisplayPassword(value);
        }
    }

    public bool WindowStackable
    {
        get
        {
            return false;
        }
    }

    public bool WindowSingleton
    {
        get { return true; }
    }

    public float WindowDepth
    {
        get { return tagalongScript.TagalongDistance; }
        set { tagalongScript.TagalongDistance = value; }
    }

    private string DisplayPassword(string password)
    {
        string res = "";
        for (int i=0;i<password.Length;i++)
        {
            res += "*";
        }
        return res;
    }

    // Use this for initialization
    void Start()
    {
        Button userNameButton = userNameInputField.AddComponent<Button>();
        userNameButton.OnPressed = InputUserName;

        Button passwordButton = passwordInputField.AddComponent<Button>();
        passwordButton.OnPressed = InputPassword;

        Button confirmButton = confirmButtonObject.AddComponent<Button>();
        confirmButton.OnPressed = Login;

        userNameCaption = userNameInputText.GetComponent<TextMesh>();
        passwordCaption = passwordInputText.GetComponent<TextMesh>();

        tagalongScript = GetComponent<SimpleTagalong>();

        WindowManager.Instance.Add(this);
    }

    private void Login()
    {
        Debug.Log("Logged in");
        AuthorizationManager.Authorize(UserName, Password);
    }

    private void InputPassword()
    {
        Keyboard.Display(LocalizationManager.Instance.ResolveString("Enter the password"), SetPassword, true);
    }

    private void SetPassword(string pwd)
    {
        if (pwd != null)
        {
            Password = pwd;
        }
    }

    private void InputUserName()
    {
        Keyboard.Display(LocalizationManager.Instance.ResolveString("Enter the user name"), UserName, SetUserName, true); 
    }

    private void SetUserName(string name)
    {
        if (name != null)
        {
            UserName = name;
        }
    }

    public void Cancel()
    {
        Destroy(gameObject);
    }

    public void CloseWindow()
    {
        Cancel();
    }
}
