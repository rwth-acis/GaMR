using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnnotationBox : MonoBehaviour
{

    public GameObject editButton, deleteButton, closeButton;
    public GameObject textField;
    private Button buttonEdit, buttonDelete, buttonClose;
    private Caption caption;

    public static AnnotationBox currentlyOpenAnnotationBox;


    public AnnotationContainer container;

    // Use this for initialization
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

    private void EditText()
    {
        Keyboard.Display("Edit the annotation", container.Annotation.Text, OnEditFinished, true);
        gameObject.SetActive(false);

    }

    private void OnEditFinished(string input)
    {
        if (input != null)
        {
            container.Annotation.Text = input;
            caption.Text = input;
        }
        gameObject.SetActive(true);
    }

    private void DeleteAnnotation()
    {
        container.DeleteAnnotation();
        Close();
    }

    public void Close()
    {
        currentlyOpenAnnotationBox = null;
        Destroy(gameObject);
    }

    public static void Show(AnnotationContainer container)
    {
        GameObject instance = (GameObject)GameObject.Instantiate(Resources.Load("AnnotationBox"));
        AnnotationBox annotationBox = instance.GetComponent<AnnotationBox>();
        annotationBox.container = container;
        currentlyOpenAnnotationBox = annotationBox;
    }
}
