using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Annotation {

    private Vector3 position;
    private Vector3 up;
    private string text;

    public Annotation(Vector3 position, string text)
    {
        this.position = position;
        this.text = text;
    }

    public Vector3 Position
    {
        get { return position; }
    }

    public string Text
    {
        get { return text; }
    }
}
