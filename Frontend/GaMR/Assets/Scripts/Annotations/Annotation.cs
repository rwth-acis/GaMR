using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Annotation {

    [SerializeField]
    private Vector3 position;
    [SerializeField]
    private string text;

    public Annotation(Vector3 position, string text)
    {
        this.position = position;
        this.text = text;
    }

    public Annotation(Vector3 position)
    {
        this.position = position;
        this.text = "";
    }

    public Vector3 Position
    {
        get { return position; }
    }

    public string Text
    {
        get { return text; }
        set { text = value; }
    }
}
