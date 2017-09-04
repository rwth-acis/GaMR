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
        base.Start();
        actions = BoundingBox.GetComponent<BoundingBoxActions>();
        InitializeButtons();
        SetPlayerTypeMode();
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
        deleteButton = transform.Find("Delete Button").gameObject.AddComponent<FocusableButton>();
        editModeButton = transform.Find("EditMode Button").gameObject.AddComponent<FocusableCheckButton>();
        boundingBoxButton = transform.Find("Box Button").gameObject.AddComponent<FocusableCheckButton>();
        quizButton = transform.Find("Quiz Button").gameObject.AddComponent<FocusableCheckButton>();
        createQuizButton = transform.Find("CreateQuiz Button").gameObject.AddComponent<FocusableButton>();
        closeButton = transform.Find("Close Button").gameObject.AddComponent<FocusableButton>();

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

    private void Destroy()
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
