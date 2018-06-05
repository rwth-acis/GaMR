using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ressource collection where the gameobjects of the windows can be collected
/// This is mainly used for consistent spawning of the different menu types.
/// If a window resource is exchanged it only needs to be changed here
/// </summary>
public class WindowResources : Singleton<WindowResources>
{

    [SerializeField] private GameObject loginMenu;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject loginSelectionMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject languageMenu;
    [SerializeField] private GameObject contextMenu;
    [SerializeField] private GameObject quizMenu;
    [SerializeField] private GameObject quizSelectionMenu;
    [SerializeField] private GameObject badgeOverviewMenu;
    [SerializeField] private GameObject annotationMenu;
    [SerializeField] private GameObject thumbnail;
    [SerializeField] private GameObject keyboard;
    [SerializeField] private GameObject numpad;
    [SerializeField] private GameObject focusableButton;

    public GameObject LoginMenu
    {
        get {return loginMenu;}
    }

    public GameObject MainMenu
    {
        get {return mainMenu;}
    }

    public GameObject LoginSelectionMenu
    {
        get { return loginSelectionMenu; }
    }

    public GameObject SettingsMenu
    {
        get { return settingsMenu; }
    }

    public GameObject LanguageMenu
    {
        get { return languageMenu; }
    }

    public GameObject ContextMenu
    {
        get { return contextMenu; }
    }

    public GameObject QuizMenu
    {
        get { return quizMenu; }
    }

    public GameObject QuizSelectionMenu
    {
        get { return quizSelectionMenu; }
    }

    public GameObject BadgeOverviewMenu
    {
        get { return badgeOverviewMenu; }
    }

    public GameObject AnnotationMenu
    {
        get { return annotationMenu; }
    }

    public GameObject Thumbnail
    {
        get { return thumbnail; }
    }

    public GameObject Keyboard
    {
        get { return keyboard; }
    }

    public GameObject Numpad
    {
        get { return numpad; }
    }

    public GameObject FocusableButton
    {
        get { return focusableButton; }
    }
}
