using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AnnotationContainer : MonoBehaviour, IInputHandler
{

    public AnnotationManager annotationManager;
    public bool loaded;


    public void Start()
    {
        if (!loaded)
        {
            Keyboard.Display("Enter the text of the annotation", UserInputFinished, true);
        }
    }

    private void UserInputFinished(string input)
    {
        if (input == null) // user canceled
        {
            Destroy(gameObject);
        }
        else
        {
            Annotation = new Annotation(transform.position, input);
            annotationManager.Add(Annotation);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnInputUp(InputEventData eventData)
    {
    }

    public void OnInputDown(InputEventData eventData)
    {
        // close the keyboard if it is open
        if (Keyboard.currentlyOpenedKeyboard != null)
        {
            Keyboard.currentlyOpenedKeyboard.Cancel();
        }


        // make sure that only one annotation box is opened and that it is opened for this annotation
        if (AnnotationBox.currentlyOpenAnnotationBox != null)
        {
            if (AnnotationBox.currentlyOpenAnnotationBox.container != this)
            {
                AnnotationBox.currentlyOpenAnnotationBox.Close();
                AnnotationBox.Show(this);
            }
        }
        else
        {
            AnnotationBox.Show(this);
        }
    }

    public Annotation Annotation
    {
        get; set;
    }

    public void DeleteAnnotation()
    {
        annotationManager.Delete(Annotation);
        Destroy(gameObject);
    }
}
