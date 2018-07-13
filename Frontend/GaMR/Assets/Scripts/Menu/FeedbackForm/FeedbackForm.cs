using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackForm : BaseMenu
{
    FocusableContentButton titleField;
    FocusableContentButton commentField;
    FocusableButton postOnReqBaz;

    private bool menuEnabled = true;

    private string title;
    private string comment;

    public Action OnCloseAction
    { get; set; }

    public bool MenuEnabled
    {
        get { return menuEnabled; }
        set
        {
            menuEnabled = value;
            titleField.ButtonEnabled = value;
            commentField.ButtonEnabled = value;
            postOnReqBaz.ButtonEnabled = value;
        }
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        InitializeButtons();
    }

    private void InitializeButtons()
    {
        // get/create buttons
        titleField = transform.Find("Title Button").gameObject.GetComponent<FocusableContentButton>();
        commentField = transform.Find("Comment Button").gameObject.GetComponent<FocusableContentButton>();
        postOnReqBaz = transform.Find("Post ReqBaz Button").gameObject.GetComponent<FocusableButton>();

        // set button actions
        postOnReqBaz.OnPressed = Close;
        titleField.OnPressed = () => { Keyboard.Display("Enter title", title, TitleSet, true); };
        commentField.OnPressed = () => { Keyboard.Display("Enter your comment", comment, CommentSet, true); };
        

        OnUpdateLanguage();
    }

    private void Close()
    {
        if (OnCloseAction != null)
        {
            OnCloseAction();
        }
        Destroy(gameObject);
    }

    public override void OnUpdateLanguage()
    {
        // set captions
        titleField.Text = LocalizationManager.Instance.ResolveString("Title");
        commentField.Text = LocalizationManager.Instance.ResolveString("Comment");
        postOnReqBaz.Text = LocalizationManager.Instance.ResolveString("Post on Requirements Bazaar");
    }

    private void TitleSet(string title)
    {
        if (title != null)
        {
            this.title = title;
        }
    }

    private void CommentSet(string comment)
    {
        if (comment != null)
        {
            this.comment = comment;
        }
    }
}
