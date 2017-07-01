using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AnnotationContainer : MonoBehaviour, IInputHandler {

    public AnnotationManager annotationManager;
    private GameObject annotationBoxInstance;


    public void Start()
    {
        Keyboard.Display("Enter the text of the annotation", UserInputFinished, true);
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
    void Update () {
		
	}

    public void OnInputUp(InputEventData eventData)
    {
    }

    public void OnInputDown(InputEventData eventData)
    {
        if (annotationBoxInstance == null)
        {
            annotationBoxInstance = AnnotationBox.Show(this);
        }
    }

    public Annotation Annotation
    {
        get;set;
    }

    public void DeleteAnnotation()
    {
        annotationManager.Delete(Annotation);
        Destroy(gameObject);
    }
}
