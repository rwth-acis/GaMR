using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// logically represents an annotation
/// </summary>
[Serializable]
public class Annotation {

    [SerializeField]
    private Vector3 position;
    [SerializeField]
    private string text;
    public bool Answered { get; set; }

    /// <summary>
    /// creates an annotation
    /// </summary>
    /// <param name="position">The position of the annotation</param>
    /// <param name="text">The text of the annotation</param>
    public Annotation(Vector3 position, string text)
    {
        this.position = position;
        this.text = text;
    }

    /// <summary>
    /// creates an annotation
    /// </summary>
    /// <param name="position">The position of the annotation</param>
    public Annotation(Vector3 position)
    {
        this.position = position;
        this.text = "";
    }

    /// <summary>
    /// The position of the annotation
    /// </summary>
    public Vector3 Position
    {
        get { return position; }
    }

    /// <summary>
    /// The text which the annotation stores
    /// </summary>
    public string Text
    {
        get { return text; }
        set { text = value; }
    }

    public string PositionToStringWithoutDots
    {
        get
        {
            string posString = Position.ToString();
            posString = posString.Replace("(", "");
            posString = posString.Replace(")", "");
            posString = posString.Replace(".", "-");
            return posString;
        }
    }
}
