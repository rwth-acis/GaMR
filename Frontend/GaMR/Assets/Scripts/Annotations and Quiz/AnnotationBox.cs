using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The display-window which is shown if an annotation is selected
/// </summary>
public class AnnotationBox : MonoBehaviour
{

    public GameObject editButton, deleteButton, closeButton;
    public GameObject textField;
    private Button buttonEdit, buttonDelete, buttonClose;
    private Caption caption;

    public static AnnotationBox currentlyOpenAnnotationBox;


    public AnnotationContainer container;

    /// <summary>
    /// initializes the annotation box
    /// assigns all buttons
    /// fills it with the text of the annotation
    /// </summary>
    void Start()
    {

        buttonEdit = editButton.GetComponent<Button>();
        buttonDelete = deleteButton.GetComponent<Button>();
        buttonClose = closeButton.GetComponent<Button>();
        caption = textField.GetComponent<Caption>();

        // necessary since caption has not yet called Start() but is needed immediately
        caption.Init();
        caption.Text = container.Annotation.Text;

        buttonEdit.OnPressed = EditText;
        buttonDelete.OnPressed = DeleteAnnotation;
        buttonClose.OnPressed = Close;
    }

    /// <summary>
    /// Called if the edit-button is pressed => opens a keyboard to edit the annotation
    /// </summary>
    private void EditText()
    {
        Keyboard.Display("Edit the annotation", container.Annotation.Text, OnEditFinished, true);
        gameObject.SetActive(false);

    }

    /// <summary>
    /// called if the edit-keyboard is closed
    /// applies changes to the annotation-text
    /// </summary>
    /// <param name="input">The text which was typed by the user (null if input was cancelled)</param>
    private void OnEditFinished(string input)
    {
        if (input != null)
        {
            container.EditAnnotation(input);
            caption.Text = input;
        }
        gameObject.SetActive(true);

    }

    /// <summary>
    /// called if the delete-button is pressed
    /// deletes the annotation
    /// </summary>
    private void DeleteAnnotation()
    {
        container.DeleteAnnotation();
        Close();
    }

    /// <summary>
    /// called if the close-button is pressed
    /// closes the AnnotationBox
    /// </summary>
    public void Close()
    {
        container.Deselect();
        currentlyOpenAnnotationBox = null;
        Destroy(gameObject);
    }

    /// <summary>
    /// Creates an annotation box and shows it
    /// </summary>
    /// <param name="container"></param>
    public static void Show(AnnotationContainer container)
    {
        GameObject instance = (GameObject)GameObject.Instantiate(Resources.Load("AnnotationBox"));
        AnnotationBox annotationBox = instance.GetComponent<AnnotationBox>();
        annotationBox.container = container;
        container.Select();
        currentlyOpenAnnotationBox = annotationBox;
    }
}
