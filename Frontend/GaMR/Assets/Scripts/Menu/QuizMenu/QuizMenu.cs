using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizMenu : BaseMenu, IViewEvents
{

    FocusableCheckButton itemButton1, itemButton2, itemButton3, itemButton4, itemButton5;
    FocusableButton pageDownButton, pageUpButton;
    List<FocusableCheckButton> buttons = new List<FocusableCheckButton>();
    QuizManager quizManager;
    int startIndex = 0;
    int questionSelected = -1;


    public GameObject BoundingBox { get; set; }

    public Action OnCloseAction
    { get; set; }

    protected override void Start()
    {
        base.Start();
        questionSelected = -1;
        quizManager = BoundingBox.GetComponentInChildren<QuizManager>();
        quizManager.AddListener(this);
        InitializeButtons();
    }

    private void InitializeButtons()
    {
        itemButton1 = transform.Find("Item1 Button").gameObject.AddComponent<FocusableCheckButton>();
        itemButton2 = transform.Find("Item2 Button").gameObject.AddComponent<FocusableCheckButton>();
        itemButton3 = transform.Find("Item3 Button").gameObject.AddComponent<FocusableCheckButton>();
        itemButton4 = transform.Find("Item4 Button").gameObject.AddComponent<FocusableCheckButton>();
        itemButton5 = transform.Find("Item5 Button").gameObject.AddComponent<FocusableCheckButton>();

        buttons.Add(itemButton1);
        buttons.Add(itemButton2);
        buttons.Add(itemButton3);
        buttons.Add(itemButton4);
        buttons.Add(itemButton5);

        for (int i=0;i<buttons.Count;i++)
        {
            buttons[i].OnButtonPressed = OnQuestionSelected;
        }

        pageDownButton = transform.Find("Down Button").gameObject.AddComponent<FocusableButton>();
        pageUpButton = transform.Find("Up Button").gameObject.AddComponent<FocusableButton>();

        pageDownButton.OnPressed = PageDown;
        pageUpButton.OnPressed = PageUp;


        OnUpdateLanguage();

        FillQuestions();
    }

    private void OnQuestionSelected(Button sender)
    {
        int nButton = questionSelected - startIndex;
        if (nButton >= 0 && nButton < buttons.Count)
        {
            buttons[nButton].ButtonChecked = false;
        }
        questionSelected = sender.Data;
        ((FocusableCheckButton)sender).ButtonChecked = true;
        if (quizManager.PositionToName)
        {
            quizManager.EvaluateQuestion(quizManager.Annotations[questionSelected].Text);
        }
        else
        {
            quizManager.CurrentlySelectedName = quizManager.Annotations[questionSelected].Text;
        }
    }

    public override void OnUpdateLanguage()
    {
        // localization:
        pageUpButton.Text = LocalizationManager.Instance.ResolveString("Page up");
        pageDownButton.Text = LocalizationManager.Instance.ResolveString("Page down");
    }

    private void FillQuestions()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            int iModel = i + startIndex;
            if (iModel < quizManager.Annotations.Count)
            {
                buttons[i].Visible = true;
                buttons[i].Text = quizManager.Annotations[iModel].Text;
                buttons[i].ButtonEnabled = !quizManager.Annotations[iModel].Answered;
                buttons[i].ButtonChecked = (questionSelected == iModel);
                buttons[i].Data = iModel;
            }
            else
            {
                buttons[i].Visible = false;
            }
        }

        SetButtonStates();
    }

    private void PageDown()
    {
        Debug.Log("Down");
        startIndex += buttons.Count;
        FillQuestions();
    }

    private void PageUp()
    {
        Debug.Log("Up");
        startIndex -= buttons.Count;
        FillQuestions();
    }

    private void SetButtonStates()
    {
        if (quizManager.Annotations.Count > startIndex + buttons.Count)
        {
            pageDownButton.ButtonEnabled = true;
        }
        else
        {
            pageDownButton.ButtonEnabled = false;
        }

        if (startIndex > 0)
        {
            pageUpButton.ButtonEnabled = true;
        }
        else
        {
            pageUpButton.ButtonEnabled = false;
        }
    }

    public void UpdateView()
    {
        FillQuestions();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        UpdateView();
    }

    public void Close()
    {
        questionSelected = -1;
        gameObject.SetActive(false);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    protected override void OnDestroy()
    {
        quizManager.RemoveListener(this);
        base.OnDestroy();
    }
}
