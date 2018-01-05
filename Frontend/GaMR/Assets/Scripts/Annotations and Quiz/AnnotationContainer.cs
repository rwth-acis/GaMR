using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Container which encapulates an annotation so that it can be attached to a gameobject
/// </summary>
public class AnnotationContainer : MonoBehaviour, IInputHandler
{

    public AnnotationManager annotationManager;
    public bool loaded;

    private Material mat;

    private Color deselectedColor;
    public Color selectedColor = Color.red;

    private bool isQuiz;

    /// <summary>
    /// initializes the container
    /// if it was not created from the load-routine a keyboard automatically appears
    /// to fill the annotation with text
    /// also: the annotation appears as selected
    /// </summary>
    public void Start()
    {
        deselectedColor = GetComponent<Renderer>().material.color;
        mat = gameObject.GetComponent<Renderer>().material;
        if (!loaded)
        {
            mat.color = selectedColor;
            Keyboard.Display(LocalizationManager.Instance.ResolveString("Enter the text of the annotation"), UserInputFinished, true);
        }
        if (annotationManager.GetType() == typeof(QuizManager))
        {
            Debug.Log("I am a quiz!");
            isQuiz = true;
        }
        else
        {
            isQuiz = false;
        }
    }

    /// <summary>
    /// Called when the user has finished the input on the keyboard
    /// </summary>
    /// <param name="input">The input of the user (null if input was cancelled)</param>
    private void UserInputFinished(string input)
    {
        // if input cancelled => destroy the annotation object again
        if (input == null) // user canceled
        {
            Destroy(gameObject);
        }
        else // create an annotation and add it to the annotation-manager
        {
            // deselect the object since editing has finished
            mat.color = deselectedColor;
            Annotation = new Annotation(transform.localPosition, input);
            annotationManager.Add(this);
        }
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

        // show the annotation-edit box if it is in edit mode
        if (!isQuiz || (isQuiz && annotationManager.EditMode))
        {
            GameObject annotationMenuInstance = Instantiate(WindowResources.Instance.AnnotationMenu);
            annotationMenuInstance.GetComponent<CirclePositioner>().boundingBox = transform.parent.parent;
            annotationMenuInstance.GetComponent<AnnotationMenu>().Container = this;

        }
        // else: it is a quiz
        else
        {
            // determine which direction was asked
            if (((QuizManager)annotationManager).PositionToName)
            {
                // Keyboard.Display(LocalizationManager.Instance.ResolveString("How is this part called?"), UserQuizInputFinished, true);
                ((QuizManager)annotationManager).CurrentlySelectedAnnotation = Annotation;
                ((QuizManager)annotationManager).ShowNames();
                Select();
                ((QuizManager)annotationManager).CurrentlySelectedAnnotationContainer = this;
            }
            else
            {
                ((QuizManager)annotationManager).EvaluateQuestion(Annotation);
            }
        }
    }

    /// <summary>
    /// The annotation object
    /// </summary>
    public Annotation Annotation
    {
        get; set;
    }

    /// <summary>
    /// deletes the annotation's gameobject and the annotation in the annotation-manager
    /// </summary>
    public void DeleteAnnotation()
    {
        annotationManager.Delete(this);
        Destroy(gameObject);
    }

    public void EditAnnotation(string newText)
    {
        // set text
        Annotation.Text = newText;
        // notify annotation manager that a change happened
        annotationManager.NotifyAnnotationEdited(Annotation);
    }

    public void Select()
    {
        mat.color = new Color(selectedColor.r, selectedColor.g, selectedColor.b, mat.color.a);
    }

    internal void Deselect()
    {
        mat.color = new Color(deselectedColor.r, deselectedColor.g, deselectedColor.b, mat.color.a);
    }
}
