using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public CanvasScaler canvasScaler;
    public Transform loginPage;
    public Transform settingsPage;

    private Animator loginPageAnimator;
    private Animator settingsPageAnimator;

    private void Awake()
    {
        loginPageAnimator = loginPage.GetComponent<Animator>();
        settingsPageAnimator = settingsPage.GetComponent<Animator>();
    }

    public void Login()
    {
        AuthorizationManager.Instance.Login();
    }

    public void ShowSettings()
    {
        loginPageAnimator.SetBool("LoginMenuVisible", !loginPageAnimator.GetBool("LoginMenuVisible"));
        settingsPageAnimator.SetBool("SettingsVisible", !settingsPageAnimator.GetBool("SettingsVisible"));
    }

    private void Update()
    {
    }

}
