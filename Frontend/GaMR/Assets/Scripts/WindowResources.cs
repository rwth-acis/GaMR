using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ressource collection where the gameobjects of the windows can be collected
/// This is mainly used for consistent spawning of the different menu types.
/// If a menu gameobject is exchanged it only needs to be changed here
/// </summary>
public class WindowResources : Singleton<WindowResources>
{

    [SerializeField] private GameObject loginMenu;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject languageMenu;
    [SerializeField] private GameObject contextMenu;
    [SerializeField] private GameObject quizMenu;
    [SerializeField] private GameObject badgeOverviewMenu;
    [SerializeField] private GameObject annotationMenu;
    [SerializeField] private GameObject thumbnail;

    public GameObject LoginMenu
    {
        get {return loginMenu;}
    }

    public GameObject MainMenu
    {
        get {return mainMenu;}
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
}
