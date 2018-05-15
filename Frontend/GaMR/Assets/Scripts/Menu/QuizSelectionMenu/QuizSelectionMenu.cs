using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizSelectionMenu : BaseMenu, IViewEvents
{

    FocusableButton itemButton1, itemButton2, itemButton3, itemButton4, itemButton5;
    FocusableButton pageDownButton, pageUpButton;
    List<FocusableButton> buttons = new List<FocusableButton>();
    int startIndex = 0;
    int quizSelected = -1;


    public GameObject BoundingBox { get; set; }

    public List<string> Items { get; set; }

    public Action<bool, string> OnCloseAction // bool: was a quiz selected?, string: name of the quiz
    { get; set; }

    protected override void Start()
    {
        base.Start();
        if (Items == null)
        {
            Items = new List<string>();
        }
        quizSelected = -1;
        InitializeButtons();
    }

    private void InitializeButtons()
    {
        itemButton1 = transform.Find("Item1 Button").gameObject.GetComponent<FocusableButton>();
        itemButton2 = transform.Find("Item2 Button").gameObject.GetComponent<FocusableButton>();
        itemButton3 = transform.Find("Item3 Button").gameObject.GetComponent<FocusableButton>();
        itemButton4 = transform.Find("Item4 Button").gameObject.GetComponent<FocusableButton>();
        itemButton5 = transform.Find("Item5 Button").gameObject.GetComponent<FocusableButton>();

        buttons.Add(itemButton1);
        buttons.Add(itemButton2);
        buttons.Add(itemButton3);
        buttons.Add(itemButton4);
        buttons.Add(itemButton5);

        for (int i=0;i<buttons.Count;i++)
        {
            buttons[i].OnButtonPressed = OnQuizSelected;
        }

        pageDownButton = transform.Find("Down Button").gameObject.GetComponent<FocusableButton>();
        pageUpButton = transform.Find("Up Button").gameObject.GetComponent<FocusableButton>();

        pageDownButton.OnPressed = PageDown;
        pageUpButton.OnPressed = PageUp;


        OnUpdateLanguage();

        FillQuizzes();
    }

    private void OnQuizSelected(GaMRButton sender)
    {
        quizSelected = sender.Data;
        Close();
    }

    public override void OnUpdateLanguage()
    {
        // localization:
        pageUpButton.Text = LocalizationManager.Instance.ResolveString("Page up");
        pageDownButton.Text = LocalizationManager.Instance.ResolveString("Page down");
    }

    private void FillQuizzes()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            int iModel = i + startIndex;
            if (iModel < Items.Count)
            {
                buttons[i].Visible = true;
                buttons[i].Text = Items[i];
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
        FillQuizzes();
    }

    private void PageUp()
    {
        Debug.Log("Up");
        startIndex -= buttons.Count;
        FillQuizzes();
    }

    private void SetButtonStates()
    {
        if (Items.Count > startIndex + buttons.Count)
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
        FillQuizzes();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        UpdateView();
    }

    public void Close()
    {
        if (OnCloseAction != null)
        {
            if (quizSelected != -1) // a quiz was selected => index is not -1 anymore
            {
                OnCloseAction(true, Items[quizSelected]);
            }
            else
            {
                OnCloseAction(false, null);
            }
        }
        quizSelected = -1; // reset selected quiz index for next selection
        gameObject.SetActive(false); // deactivate view
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}
