﻿using HoloToolkit.Unity.InputModule;
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
            Keyboard.Display("Enter the text of the annotation", UserInputFinished, true);
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

    public void Select()
    {
        mat.color = new Color(selectedColor.r, selectedColor.g, selectedColor.b, mat.color.a);
    }

    internal void Deselect()
    {
        mat.color = new Color(deselectedColor.r, deselectedColor.g, deselectedColor.b, mat.color.a);
    }
}