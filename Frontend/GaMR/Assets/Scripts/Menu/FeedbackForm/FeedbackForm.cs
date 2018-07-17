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

    private string title = "";
    private string comment = "";

    private TextMesh titleTextMesh;
    private TextMesh commentTextMesh;
    private float titleMaxWidth;
    private float commentMaxWidth;

    public Action OnCloseAction
    { get; set; }

    private string Title
    {
        get
        {
            return title;
        }
        set
        {
            title = value;
            string contentWithLineBreaks = AutoLineBreak.StringWithLineBreaks(titleTextMesh, title, titleMaxWidth);
            string shownContent = contentWithLineBreaks;
            string[] lines = contentWithLineBreaks.Split('\n');
            if (lines.Length > 2)
            {
                lines[1].Remove(lines[1].Length - 4);
                lines[1] += "...";
                shownContent = lines[0] + '\n' + lines[1];
            }
            titleField.Content = shownContent;
        }
    }

    private string Comment
    {
        get
        {
            return comment;
        }
        set
        {
            comment = value;
            string contentWithLineBreaks = AutoLineBreak.StringWithLineBreaks(commentTextMesh, comment, commentMaxWidth);
            string shownContent = contentWithLineBreaks;
            string[] lines = contentWithLineBreaks.Split('\n');
            if (lines.Length > 5)
            {
                lines[4].Remove(lines[1].Length - 4);
                lines[4] += "...";
                shownContent = lines[0] + '\n' + lines[1] + '\n' + lines[2] + '\n' + lines[3] + '\n' + lines[4];
            }
            commentField.Content = shownContent;
        }
    }

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

        GetFieldData();
    }

    /// <summary>
    /// Gets the required components and calculates the maximum widths for the input fields
    /// </summary>
    private void GetFieldData()
    {
        // for the title field:
        titleTextMesh = titleField.transform.Find("Content").GetComponent<TextMesh>();

        Quaternion originalRotation = titleField.transform.rotation;
        titleField.transform.rotation = Quaternion.identity;
        // overall width of the whole input field:
        float fieldWidth = titleField.GetComponent<Collider>().bounds.size.z;
        // the overall width is too long since the text part only starts with a margin
        float distanceBetweenButtonCenterAndContent = titleTextMesh.transform.position.z - titleField.transform.position.z;
        titleField.transform.rotation = originalRotation;

        // we have the left half the text field's length; distanceBetweenButtonCenterAndContent is the part we want to have
        // subtract the remaining part for this half: it is the margin we are looking for
        titleMaxWidth = fieldWidth - (fieldWidth / 2f - distanceBetweenButtonCenterAndContent);

        // for the comment field:
        commentTextMesh = commentField.transform.Find("Content").GetComponent<TextMesh>();

        originalRotation = commentField.transform.rotation;
        commentField.transform.rotation = Quaternion.identity;
        // overall width of the whole input field:
        fieldWidth = commentField.GetComponent<Collider>().bounds.size.z;
        // the overall width is too long since the text part only starts with a margin
        distanceBetweenButtonCenterAndContent = commentTextMesh.transform.position.z - commentField.transform.position.z;
        commentField.transform.rotation = originalRotation;

        // we have the left half the text field's length; distanceBetweenButtonCenterAndContent is the part we want to have
        // subtract the remaining part for this half: it is the margin we are looking for
        commentMaxWidth = fieldWidth - (fieldWidth / 2f - distanceBetweenButtonCenterAndContent);
    }

    private void InitializeButtons()
    {
        // get/create buttons
        titleField = transform.Find("Title Button").gameObject.GetComponent<FocusableContentButton>();
        commentField = transform.Find("Comment Button").gameObject.GetComponent<FocusableContentButton>();
        postOnReqBaz = transform.Find("Post ReqBaz Button").gameObject.GetComponent<FocusableButton>();

        // set button actions
        postOnReqBaz.OnPressed = SubmitToReqBaz;
        titleField.OnPressed = () => { Keyboard.Display("Enter title", title, TitleSet, true); };
        commentField.OnPressed = () => { Keyboard.Display("Enter your comment", comment, CommentSet, true); };
        

        OnUpdateLanguage();
    }

    private void SubmitToReqBaz()
    {
        // Requirements Bazaar uses a different access token header format => include it
        RestManager.Instance.StandardHeader.Add("Authorization", "Bearer " + AuthorizationManager.Instance.AccessToken);

        RequirementPost post = new RequirementPost(Title, Comment, 376, new CategoryId[] { new CategoryId(692) });

        post.categories = new CategoryId[] { new CategoryId(692) };

        string json = JsonUtility.ToJson(post);

        RestManager.Instance.POST("https://requirements-bazaar.org/bazaar/requirements", json);

        RestManager.Instance.StandardHeader.Remove("Authorization"); // remove different header again
        MessageBox.Show("Thank you for your feedback!", MessageBoxType.SUCCESS);
        Close();
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
        postOnReqBaz.Text = LocalizationManager.Instance.ResolveString("Post on\nRequirements Bazaar");
    }

    private void TitleSet(string title)
    {
        if (title != null)
        {
            Title = title;
        }
    }

    private void CommentSet(string comment)
    {
        if (comment != null)
        {
            Comment = comment;
        }
    }
}
