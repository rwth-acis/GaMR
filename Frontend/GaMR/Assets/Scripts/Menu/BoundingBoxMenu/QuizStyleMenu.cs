using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizStyleMenu : BaseMenu
{

    public Action<bool> OnCloseAction
    { get; set; }
    private BoundingBoxMenu boxMenu;

    private FocusableButton cancelButton;
    private FocusableButton positionToNameButton;
    private FocusableButton nameToPositionButton;
    private FocusableButton randomButton;

    private BoundingBoxActions actions;

    protected override void Start()
    {
        base.Start();
        boxMenu = transform.parent.GetComponent<BoundingBoxMenu>();
        actions = boxMenu.BoundingBox.GetComponent<BoundingBoxActions>();
        InitializeButtons();
    }

    private void InitializeButtons()
    {
        cancelButton = transform.Find("Cancel Button").gameObject.GetComponent<FocusableButton>();
        positionToNameButton = transform.Find("PositionName Button").gameObject.GetComponent<FocusableButton>();
        nameToPositionButton = transform.Find("NamePosition Button").gameObject.GetComponent<FocusableButton>();
        randomButton = transform.Find("Random Button").gameObject.GetComponent<FocusableButton>();

        cancelButton.OnPressed = () => { Close(false); };
        positionToNameButton.OnPressed = () => { actions.SelectQuizPositionToName(); Close(true); };
        nameToPositionButton.OnPressed = () => { actions.SelectQuizNameToPosition(); Close(true); };
        randomButton.OnPressed = () => { actions.SelectQuizRandom(); Close(true); };

        OnUpdateLanguage();

    }

    private void Close(bool successulQuizSelection)
    {
        gameObject.SetActive(false);
        boxMenu.MenuEnabled = true;
        if (OnCloseAction != null)
        {
            OnCloseAction(successulQuizSelection);
        }
    }

    public override void OnUpdateLanguage()
    {
        cancelButton.Text = LocalizationManager.Instance.ResolveString("Cancel");
        positionToNameButton.Text = LocalizationManager.Instance.ResolveString("Position - Name");
        nameToPositionButton.Text = LocalizationManager.Instance.ResolveString("Name - Position");
        randomButton.Text = LocalizationManager.Instance.ResolveString("Random");
    }
}
