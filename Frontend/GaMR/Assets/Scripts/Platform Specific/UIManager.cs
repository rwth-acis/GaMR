using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public CanvasScaler canvasScaler;
    public Transform loginPage;
    public Transform settingsPage;
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

    public void ShowLanguages()
    {
        uiAnimator.SetBool("ShowLanguages", true);
    }

    public void SetLanguage(int index)
    {
        if (index == 1)
        {
            InformationManager.Instance.Language = Language.GERMAN;
        }
        else if (index == 2)
        {
            InformationManager.Instance.Language = Language.DUTCH;
        }
        else
        {
            InformationManager.Instance.Language = Language.ENGLISH;
        }

        // return to the login menu
        ShowLoginMenu();
    }



    public void Back()
    {
        if (uiAnimator.GetBool("ShowLanguages"))
        {
            uiAnimator.SetBool("ShowLanguages", false);
        }
        else if (uiAnimator.GetBool("ShowSettings"))
        {
            uiAnimator.SetBool("ShowSettings", false);
        }
    }

    public void ShowLoginMenu()
    {
        uiAnimator.SetBool("ShowSettings", false);
        uiAnimator.SetBool("ShowLanguages", false);
    }

    private void Update()
    {
    }
}
