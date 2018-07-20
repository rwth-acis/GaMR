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
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject languageMenu;
    [SerializeField] private GameObject contextMenu;
    [SerializeField] private GameObject quizMenu;
    [SerializeField] private GameObject quizSelectionMenu;
    [SerializeField] private GameObject badgeOverviewMenu;
    [SerializeField] private GameObject feedbackForm;
    [SerializeField] private GameObject annotationMenu;
    [SerializeField] private GameObject thumbnail;
    [SerializeField] private GameObject keyboard;
    [SerializeField] private GameObject numpad;
    [SerializeField] private GameObject focusableButton;

    /// <summary>
    /// The login menu which is displayed first
    /// </summary>
    public GameObject LoginMenu
    {
        get {return loginMenu;}
    }

    /// <summary>
    /// The main menu which is displayed if the user has successfully logged in
    /// </summary>
    public GameObject MainMenu
    {
        get {return mainMenu;}
    }

    /// <summary>
    /// Menu where all settings like server addresses can be changed and selected
    /// </summary>
    public GameObject SettingsMenu
    {
        get { return settingsMenu; }
    }

    /// <summary>
    /// Submenu of the settings menu where the application's language can be selected from a list
    /// </summary>
    public GameObject LanguageMenu
    {
        get { return languageMenu; }
    }

    /// <summary>
    /// Context menu of a bounding box and an imported model
    /// </summary>
    public GameObject ContextMenu
    {
        get { return contextMenu; }
    }

    /// <summary>
    /// Menu where quiz questions are listed
    /// </summary>
    public GameObject QuizMenu
    {
        get { return quizMenu; }
    }

    /// <summary>
    /// Menu which contains a list of quizzes from which the user can select one
    /// </summary>
    public GameObject QuizSelectionMenu
    {
        get { return quizSelectionMenu; }
    }

    /// <summary>
    /// Menu which shows all badges that have been won by the user
    /// </summary>
    public GameObject BadgeOverviewMenu
    {
        get { return badgeOverviewMenu; }
    }

    /// <summary>
    /// feedback menu where the user can enter a title and a comment as feedback
    /// </summary>
    public GameObject FeedbackForm
    {
        get { return feedbackForm; }
    }

    /// <summary>
    /// Menu which is opened if an annotation is selected
    /// Users can change the text and audio of an annotation
    /// </summary>
    public GameObject AnnotationMenu
    {
        get { return annotationMenu; }
    }

    /// <summary>
    /// Thumbnail button displayed on the main menu
    /// Each thumbnail can display an image and a text
    /// </summary>
    public GameObject Thumbnail
    {
        get { return thumbnail; }
    }

    /// <summary>
    /// The 3D keyboard for text entry
    /// </summary>
    public GameObject Keyboard
    {
        get { return keyboard; }
    }

    /// <summary>
    /// A keyboard version which is restricted to number-entries only
    /// </summary>
    public GameObject Numpad
    {
        get { return numpad; }
    }

    /// <summary>
    /// A button
    /// </summary>
    public GameObject FocusableButton
    {
        get { return focusableButton; }
    }
}
