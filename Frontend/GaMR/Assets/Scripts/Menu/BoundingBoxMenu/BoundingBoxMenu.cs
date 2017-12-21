using HoloToolkit.Unity.InputModule;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingBoxMenu : BaseMenu
{
    private FocusableButton deleteButton;
    private FocusableCheckButton editModeButton;
    private FocusableCheckButton boundingBoxButton;
    private FocusableCheckButton quizButton;
    private FocusableButton createQuizButton;
    private FocusableButton closeButton;

    private BoundingBoxActions actions;
    private BoundingBoxInfo info;
    private CustomTapToPlace tapToPlace;


    private bool menuEnabled = true;

    public bool MenuEnabled
    {
        get { return menuEnabled; }
        set
        {
            menuEnabled = value;
            deleteButton.ButtonEnabled = value;
            editModeButton.ButtonEnabled = value;
            boundingBoxButton.ButtonEnabled = value;
            quizButton.ButtonEnabled = value;
            createQuizButton.ButtonEnabled = value;
            closeButton.ButtonEnabled = value;
        }
    }
    public GameObject BoundingBox { get; set; }

    public Action OnCloseAction
    { get; set; }

    protected override void Start()
    {
        Debug.Log("Start of bounding box menu");
        base.Start();
        actions = BoundingBox.GetComponent<BoundingBoxActions>();
        info = BoundingBox.GetComponent<BoundingBoxInfo>();
        tapToPlace = BoundingBox.GetComponent<CustomTapToPlace>();
        Debug.Log("Create menu reference for " + info.Id.ToString());
        info.Menu = this;

        Debug.Log("InfoMenu: " + info.Menu);

        InitializeButtons();
        SetPlayerTypeMode();
        gameObject.SetActive(false);

        tapToPlace.OnPickUp += BoundingBoxPickedUp;
        tapToPlace.OnPlaced += BoundingBoxPlaced;
    }

    /// <summary>
    /// if the bounding box is picked up the menu is hidden to avoid weird behavior
    /// where the bounding box is in conflict with the menu and tries to jump in front of it
    /// when the user focuses the menu
    /// </summary>
    private void BoundingBoxPickedUp()
    {
        transform.parent.gameObject.SetActive(false);
    }

    /// <summary>
    /// once the bounding box is placed using tap to place, the bounding box menu can be displayed again
    /// </summary>
    private void BoundingBoxPlaced()
    {
        transform.parent.gameObject.SetActive(true);
    }

    private void SetPlayerTypeMode()
    {
        bool isStudent = (InformationManager.Instance.playerType == PlayerType.STUDENT);
        createQuizButton.Visible = !isStudent;
        transform.Find("Top Menu").gameObject.SetActive(isStudent);
        transform.Find("Top Menu Short").gameObject.SetActive(!isStudent);
    }

    private void InitializeButtons()
    {
        deleteButton = transform.Find("Delete Button").gameObject.GetComponent<FocusableButton>();
        editModeButton = transform.Find("EditMode Button").gameObject.GetComponent<FocusableCheckButton>();
        boundingBoxButton = transform.Find("Box Button").gameObject.GetComponent<FocusableCheckButton>();
        quizButton = transform.Find("Quiz Button").gameObject.GetComponent<FocusableCheckButton>();
        createQuizButton = transform.Find("CreateQuiz Button").gameObject.GetComponent<FocusableButton>();
        closeButton = transform.Find("Close Button").gameObject.GetComponent<FocusableButton>();

        deleteButton.OnPressed = () => { actions.DeleteObject(); Destroy(); };
        editModeButton.OnPressed = ToggleEditMode;
        boundingBoxButton.OnPressed = ToggleBoundingBox;
        quizButton.OnPressed = SelectQuiz;
        closeButton.OnPressed = Close;
        createQuizButton.OnPressed = actions.CreateNewQuiz;

        boundingBoxButton.ButtonChecked = true;
        editModeButton.ButtonChecked = true;
        quizButton.ButtonChecked = false;

        OnUpdateLanguage();

    }

    private void SelectQuiz()
    {
        if (quizButton.ButtonChecked)
        {
            if (InformationManager.Instance.playerType == PlayerType.STUDENT)
            {
                MenuEnabled = false;
                QuizStyleMenu styleMenu = transform.Find("QuizStyle Menu").GetComponent<QuizStyleMenu>();
                styleMenu.OnCloseAction = (selectionSuccessful) =>
                {
                    quizButton.ButtonChecked = selectionSuccessful;
                    actions.EnableBoundingBox(false);
                    boundingBoxButton.ButtonChecked = false;

                    actions.EnableEditMode(false);
                    editModeButton.ButtonChecked = false;

                    editModeButton.ButtonEnabled = false;
                };
                styleMenu.gameObject.SetActive(true);
            }
            else
            {
                actions.EnableBoundingBox(false);
                boundingBoxButton.ButtonChecked = false;
                actions.EnableEditMode(true);
                editModeButton.ButtonChecked = true;

                actions.SelectQuiz();
            }
        }
        else
        {
            actions.AbortQuizSelection(); // in case it was still in quiz selection
            actions.LoadAnnotations();
            editModeButton.ButtonEnabled = true;
            editModeButton.ButtonChecked = true;
            actions.EnableEditMode(true);
        }
    }

    private void ToggleBoundingBox()
    {
        actions.EnableBoundingBox(boundingBoxButton.ButtonChecked);
    }

    private void ToggleEditMode()
    {
        actions.EnableEditMode(editModeButton.ButtonChecked);
    }

    private void Close()
    {
        if (OnCloseAction != null)
        {
            OnCloseAction();
        }
        gameObject.SetActive(false);
    }

    public void Destroy()
    {
        Destroy(transform.parent.gameObject);
    }

    public override void OnUpdateLanguage()
    {
        deleteButton.Text = LocalizationManager.Instance.ResolveString("Delete");
        editModeButton.Text = LocalizationManager.Instance.ResolveString("Edit Mode");
        boundingBoxButton.Text = LocalizationManager.Instance.ResolveString("Bounding Box");
        quizButton.Text = LocalizationManager.Instance.ResolveString("Quiz");
        createQuizButton.Text = LocalizationManager.Instance.ResolveString("Create Quiz");
        closeButton.Text = LocalizationManager.Instance.ResolveString("Close");
    }
}
