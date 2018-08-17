using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Feedback form where users can enter text in two text fields
/// </summary>
public class FeedbackForm : BaseMenu
{
    FocusableContentButton titleField;
    FocusableContentButton commentField;
    FocusableButton postOnReqBaz;
    FocusableButton closeButton;

    private bool menuEnabled = true;

    private string title = "";
    private string comment = "";

    private TextMesh titleTextMesh;
    private TextMesh commentTextMesh;
    private float titleMaxWidth;
    private float commentMaxWidth;

    public Action OnCloseAction
    { get; set; }

    /// <summary>
    /// The title of the feedback
    /// If set, it automatically updates the text field
    /// </summary>
    private string Title
    {
        get
        {
            return title;
        }
        set
        {
            title = value;
            // add line breaks to fit text to text field
            string contentWithLineBreaks = AutoLineBreak.StringWithLineBreaks(titleTextMesh, title, titleMaxWidth);
            string shownContent = contentWithLineBreaks;
            string[] lines = contentWithLineBreaks.Split('\n');
            // if text is too long for the text field: just cut its visualization off and show three dots
            if (lines.Length > 2)
            {
                lines[1].Remove(lines[1].Length - 4);
                lines[1] += "...";
                shownContent = lines[0] + '\n' + lines[1];
            }
            titleField.Content = shownContent;

            CheckFormComplete();
        }
    }

    /// <summary>
    /// The comment of the feedback
    /// If set, it automatically updates the comment text field
    /// </summary>
    private string Comment
    {
        get
        {
            return comment;
        }
        set
        {
            comment = value;
            // add line breaks to fit text to text field
            string contentWithLineBreaks = AutoLineBreak.StringWithLineBreaks(commentTextMesh, comment, commentMaxWidth);
            string shownContent = contentWithLineBreaks;
            string[] lines = contentWithLineBreaks.Split('\n');
            // if text is too long for the text field: just cut its visualization off and show three dots
            if (lines.Length > 5)
            {
                lines[4].Remove(lines[1].Length - 4);
                lines[4] += "...";
                shownContent = lines[0] + '\n' + lines[1] + '\n' + lines[2] + '\n' + lines[3] + '\n' + lines[4];
            }
            commentField.Content = shownContent;

            CheckFormComplete();
        }
    }

    /// <summary>
    /// Enabled and disables the menu and its buttons
    /// </summary>
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

    /// <summary>
    /// Initializes the menu and its components
    /// </summary>
    protected override void Start()
    {
        base.Start();
        InitializeButtons();

        GetFieldData();
        CheckFormComplete();
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

    /// <summary>
    /// Initializes the buttons; gets all requires references
    /// </summary>
    private void InitializeButtons()
    {
        // get/create buttons
        titleField = transform.Find("Title Button").gameObject.GetComponent<FocusableContentButton>();
        commentField = transform.Find("Comment Button").gameObject.GetComponent<FocusableContentButton>();
        postOnReqBaz = transform.Find("Post ReqBaz Button").gameObject.GetComponent<FocusableButton>();
        closeButton = transform.Find("Close Button").gameObject.GetComponent<FocusableButton>();

        // set button actions
        postOnReqBaz.OnPressed = SubmitToReqBaz;
        titleField.OnPressed = () => { Keyboard.Display( LocalizationManager.Instance.ResolveString("Enter the feedback title"), title, 50, TitleSet, true); };
        commentField.OnPressed = () => { Keyboard.Display( LocalizationManager.Instance.ResolveString("Enter your feedback"), comment, CommentSet, true); };
        closeButton.OnPressed = Close;

        OnUpdateLanguage();
    }

    /// <summary>
    /// Checks if the feedback form is complete
    /// If it is, the send-button is enabled
    /// </summary>
    private void CheckFormComplete()
    {
        if (Comment != "" && Title != "")
        {
            postOnReqBaz.ButtonEnabled = true;
        }
        else
        {
            postOnReqBaz.ButtonEnabled = false;
        }
    }

    /// <summary>
    /// Posts the feedback as a new requirement to the requirements bazaar
    /// </summary>
    private void SubmitToReqBaz()
    {
        // Requirements Bazaar uses a different access token header format => include it
        RestManager.Instance.StandardHeader.Add("Authorization", "Bearer " + AuthorizationManager.Instance.AccessToken);

        RequirementPost post = new RequirementPost(Title, Comment, 376, new CategoryId[] { new CategoryId(692) });

        post.categories = new CategoryId[] { new CategoryId(692) };

        string json = JsonUtility.ToJson(post);

        WaitCursor.Show();
        RestManager.Instance.POST("https://requirements-bazaar.org/bazaar/requirements", json, (req) =>
        {
            WaitCursor.Hide();
            if (req.isHttpError || req.isNetworkError)
            {
                MessageBox.Show(LocalizationManager.Instance.ResolveString("The feedback could not be sent. Please try again later."), MessageBoxType.ERROR);
            }
            else
            {
                MessageBox.Show(LocalizationManager.Instance.ResolveString("Thank you for your feedback!"), MessageBoxType.SUCCESS);
                Close();
            }

            RestManager.Instance.StandardHeader.Remove("Authorization"); // remove different header again
        });
    }

    /// <summary>
    /// Closes the menu
    /// </summary>
    private void Close()
    {
        if (OnCloseAction != null)
        {
            OnCloseAction();
        }
        Destroy(gameObject);
    }

    /// <summary>
    /// Called if the language is updated
    /// Assigns the translated descriptions to the menu components
    /// </summary>
    public override void OnUpdateLanguage()
    {
        // set captions
        titleField.Text = LocalizationManager.Instance.ResolveString("Title");
        commentField.Text = LocalizationManager.Instance.ResolveString("Comment");
        titleField.Content = LocalizationManager.Instance.ResolveString("Title");
        commentField.Content = LocalizationManager.Instance.ResolveString("Enter your feedback");
        postOnReqBaz.Text = LocalizationManager.Instance.ResolveString("Post on\nRequirements Bazaar");
        closeButton.Text = LocalizationManager.Instance.ResolveString("Close");
    }

    /// <summary>
    /// Called if the title has been entered using the keyboard
    /// </summary>
    /// <param name="title">The new title (null if keyboard input was aborted)</param>
    private void TitleSet(string title)
    {
        if (title != null)
        {
            Title = title;
        }
    }

    /// <summary>
    /// Called if the comment has been entered using the keyboard
    /// </summary>
    /// <param name="comment">The new comment (null if keyboard input was aborted)</param>
    private void CommentSet(string comment)
    {
        if (comment != null)
        {
            Comment = comment;
        }
    }
}
