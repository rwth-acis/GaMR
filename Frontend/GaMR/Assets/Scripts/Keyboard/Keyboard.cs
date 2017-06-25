using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keyboard : MonoBehaviour
{

    public TextMesh inputField;
    public TextMesh label;
    private string text;
    public Action<string> callWithResult;

    // Use this for initialization
    public void Start()
    {
    }

    public static void Display(string label, Action<string> callWithResult)
    {
        //Vector3 pos = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z + 3);
        GameObject numPad = (GameObject) GameObject.Instantiate(Resources.Load("NumPad"));
        Keyboard keyboard = numPad.GetComponent<Keyboard>();
        keyboard.label.text = label;
        keyboard.callWithResult = callWithResult;
    }

    public string Text
    {
        get
        {
            return text;
        }
        set
        {
            text = value;
            NotifyInputField();
        }
    }

    private void NotifyInputField()
    {
        inputField.text = Text;
    }

    public void Cancel()
    {
        if (callWithResult != null)
        {
            callWithResult(null);
        }
        GameObject.Destroy(gameObject);
    }

    public void Accept()
    {
        if (callWithResult != null)
        {
            callWithResult(text);
        }
        GameObject.Destroy(gameObject);
    }
}
