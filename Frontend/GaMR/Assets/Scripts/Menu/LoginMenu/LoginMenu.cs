using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginMenu : BaseMenu
{

    FocusableButton closeButton;
    FocusableButton learningLayersLoginButton;
    FocusableButton googleLoginButton;

    public Action OnCloseAction
    { get; set; }

    protected override void Start()
    {
        base.Start();
        InitializeButtons();
    }

    private void InitializeButtons()
    {
        // get/create buttons
        closeButton = transform.Find("Close Button").gameObject.GetComponent<FocusableButton>();
        learningLayersLoginButton = transform.Find("Learning Layers Button").gameObject.GetComponent<FocusableButton>();
        googleLoginButton = transform.Find("Google Button").gameObject.GetComponent<FocusableButton>();

        // set button actions
        closeButton.OnPressed = Close;
        learningLayersLoginButton.OnPressed = LearningLayersLogin;
        googleLoginButton.OnPressed = GoogleLogin;

        OnUpdateLanguage();
    }

    private void Close()
    {
        if (OnCloseAction != null)
        {
            OnCloseAction();
        }
        Destroy(gameObject);
    }

    private void LearningLayersLogin()
    {
        AuthorizationManager.Instance.Login(AuthorizationProvider.LEARNING_LAYERS);
        Close();
    }

    private void GoogleLogin()
    {
        AuthorizationManager.Instance.Login(AuthorizationProvider.GOOGLE);
        Close();
    }

    public override void OnUpdateLanguage()
    {
        // set captions
        closeButton.Text = LocalizationManager.Instance.ResolveString("Close");
        learningLayersLoginButton.Text = LocalizationManager.Instance.ResolveString("Sign in with Learning Layers OIDC");
    }
}
