using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizSelectionMenu : BaseMenu, IViewEvents
{

    FocusableButton itemButton1, itemButton2, itemButton3, itemButton4, itemButton5;
    FocusableButton pageDownButton, pageUpButton;
    List<FocusableButton> buttons = new List<FocusableButton>();
    QuizManager quizManager;
    int startIndex = 0;
    int quizSelected = -1;


    public GameObject BoundingBox { get; set; }

    public string[] Items { get; set; }

    public Action OnCloseAction
    { get; set; }

    protected override void Start()
    {
        base.Start();
        quizSelected = -1;
        quizManager = BoundingBox.GetComponentInChildren<QuizManager>();
        quizManager.AddListener(this);
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
        int nButton = quizSelected - startIndex;
        
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
            if (iModel < quizManager.Annotations.Count)
            {
                buttons[i].Visible = true;
                buttons[i].Text = quizManager.Annotations[iModel].Text;
                buttons[i].ButtonEnabled = !quizManager.Annotations[iModel].Answered;
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
        FillQuizzes();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        UpdateView();
    }

    public void Close()
    {
        quizSelected = -1;
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
