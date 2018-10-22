using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public CanvasScaler canvasScaler;
    public Transform loginPage;
    public Transform settingsPage;

    //private Animator loginPageAnimator;
    //private Animator settingsPageAnimator;

        public Animator uiAnimator;

    private void Awake()
    {
    }

    public void Login()
    {
        AuthorizationManager.Instance.Login();
    }

    public void ShowSettings()
    {
        uiAnimator.SetBool("ShowSettings", true);
    }

    public void ShowLoginMenu()
    {
        uiAnimator.SetBool("ShowSettings", false);
    }

    private void Update()
    {
    }

}
