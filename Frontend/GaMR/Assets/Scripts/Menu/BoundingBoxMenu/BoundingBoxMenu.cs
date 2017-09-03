using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingBoxMenu : MonoBehaviour
{
    private FocusableButton deleteButton;
    private FocusableCheckButton editModeButton;
    private FocusableCheckButton boundingBoxButton;
    private FocusableCheckButton quizButton;
    private FocusableButton createQuizButton;
    private FocusableButton closeButton;

    private BoundingBoxActions actions;

    public GameObject BoundingBox { get; set; }

    public Action OnCloseAction
    { get; set; }

    private void Start()
    {
        actions = BoundingBox.GetComponent<BoundingBoxActions>();
        InitializeButtons();
    }

    private void InitializeButtons()
    {
        deleteButton = transform.Find("Delete Button").gameObject.AddComponent<FocusableButton>();
        editModeButton = transform.Find("EditMode Button").gameObject.AddComponent<FocusableCheckButton>();
        boundingBoxButton = transform.Find("Box Button").gameObject.AddComponent<FocusableCheckButton>();
        quizButton = transform.Find("Quiz Button").gameObject.AddComponent<FocusableCheckButton>();
        createQuizButton = transform.Find("CreateQuiz Button").gameObject.AddComponent<FocusableButton>();
        closeButton = transform.Find("Close Button").gameObject.AddComponent<FocusableButton>();

        editModeButton.OnPressed = ToggleEditMode;
        boundingBoxButton.OnPressed = ToggleBoundingBox;
        deleteButton.OnPressed = () => { actions.DeleteObject(); Close(); };
        closeButton.OnPressed = Close;

        boundingBoxButton.ButtonChecked = true;
        editModeButton.ButtonChecked = true;
        quizButton.ButtonChecked = false;

        deleteButton.Text = LocalizationManager.Instance.ResolveString("Delete");
        editModeButton.Text = LocalizationManager.Instance.ResolveString("Edit Mode");
        boundingBoxButton.Text = LocalizationManager.Instance.ResolveString("Bounding Box");
        quizButton.Text = LocalizationManager.Instance.ResolveString("Quiz");
        createQuizButton.Text = LocalizationManager.Instance.ResolveString("Create Quiz");
        closeButton.Text = LocalizationManager.Instance.ResolveString("Close");

    }

    private void ToggleBoundingBox()
    {
        actions.EnableBoundingBox(!boundingBoxButton.ButtonChecked);
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
}
