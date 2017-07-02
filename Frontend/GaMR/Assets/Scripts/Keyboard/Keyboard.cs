using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keyboard : MonoBehaviour
{
    // public variables to set in the editor
    public TextMesh inputField;
    public TextMesh label;
    public int maxNumberOfLines = 10;

    // variables related to the logical keyboard-functionality
    private string text = "";
    public Action<string> callWithResult;
    private List<Key> letterKeys;
    private Key[] allKeys;
    private bool shift = true;
    private bool textInitialization = true;

    // variables concerning the size-height adaption
    private Transform inputBackgroundPivot, inputBackground;
    private int numberOfNewLines = 1, numberOfOldLines = 1;
    private Transform background, backgroundPivot;
    private float lineHeight;
    private BoxCollider coll;
    private float padding = 0.015f;

    // variables concerning the width limit
    private float maxWidth;


    public static Keyboard currentlyOpenedKeyboard;

    // Use this for initialization
    public void Start()
    {
        // get necessary components
        inputBackground = inputField.transform.parent.Find("Background");
        background = transform.Find("Background");
        coll = GetComponent<BoxCollider>();

        letterKeys = new List<Key>();
        Capslock = false;

        allKeys = GetComponentsInChildren<Key>();
        foreach (Key k in allKeys)
        {
            if (k.keyType == KeyType.LETTER)
            {
                letterKeys.Add(k);
            }
        }

        // geometry calcuations:
        // set the scaling pivots for the backgrounds
        inputBackgroundPivot = CreateScalingPivot(inputBackground);
        backgroundPivot = CreateScalingPivot(background);

        // calculate the height of one text line
        lineHeight = Geometry.GetBoundsIndependentFromRotation(inputField.transform).size.y;
        maxWidth = Geometry.GetBoundsIndependentFromRotation(inputBackground).size.x;

        // scale the input background to fit the text
        ScaleToHeight(inputBackgroundPivot, inputBackground, lineHeight + padding);
    }


    private Transform CreateScalingPivot(Transform transform)
    {
        GameObject pivotPoint = new GameObject("BackgroundScalingPivot");
        pivotPoint.transform.parent = transform.parent;
        float height = Geometry.GetBoundsIndependentFromRotation(transform).size.y;
        pivotPoint.transform.position = transform.position + new Vector3(0, height / 2, 0);
        transform.parent = pivotPoint.transform;
        return pivotPoint.transform;
    }

    public static void Display(string label, Action<string> callWithResult, bool fullKeyboard)
    {
        Display(label, "", callWithResult, fullKeyboard);
    }

    public static void Display(string label, string text, Action<string> callWithResult, bool fullKeyboard)
    {
        GameObject keyboardInstance;
        if (fullKeyboard)
        {
            keyboardInstance = (GameObject)GameObject.Instantiate(Resources.Load("Keyboard"));
        }
        else
        {
            keyboardInstance = (GameObject)GameObject.Instantiate(Resources.Load("NumPad"));
        }
        Keyboard keyboard = keyboardInstance.GetComponent<Keyboard>();
        keyboard.label.text = label;
        keyboard.Text = text;
        keyboard.callWithResult = callWithResult;
        currentlyOpenedKeyboard = keyboard;
    }

    public string Text
    {
        get
        {
            return text;
        }
        set
        {
            numberOfNewLines = value.Split('\n').Length;
            if (numberOfNewLines <= maxNumberOfLines)
            {
                text = value;
                // update the input field
                NotifyInputField();
                // also handle one-time shift: use lower-case again after one letter
                // do not change shift if text is initialized by the program
                if (!textInitialization)
                {
                    if (!Capslock && Shift)
                    {
                        Shift = false;
                    }
                }
                else
                {
                    textInitialization = false;
                }
                // check and update the sizes of the backgrounds
                if (numberOfOldLines != numberOfNewLines)
                {
                    UpdateHeights();
                    // update the line number
                    numberOfOldLines = numberOfNewLines;
                }
            }
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
        Close();
    }

    private void Close()
    {
        currentlyOpenedKeyboard = null;
        Destroy(gameObject);
    }


    private void UpdateHeights()
    {
        int dirFac;
        dirFac = Math.Sign(numberOfNewLines - numberOfOldLines);
        float newTextHeight = Geometry.GetBoundsIndependentFromRotation(inputField.transform).size.y;
        ScaleToHeight(inputBackgroundPivot, inputBackground, newTextHeight + padding);
        foreach (Key k in allKeys)
        {
            k.transform.position -= dirFac * new Vector3(0, lineHeight, 0);
        }

        float backgroundHeight = Geometry.GetBoundsIndependentFromRotation(background).size.y;
        ScaleToHeight(backgroundPivot, background, backgroundHeight + dirFac * lineHeight);
        coll.size = new Vector3(
            coll.size.x,
            backgroundPivot.localScale.y * background.localScale.y,
            coll.size.z);
        coll.center = background.position - transform.position;
    }

    private void ScaleToHeight(Transform pivot, Transform trans, float height)
    {
        Transform parent = pivot.parent;
        pivot.parent = null;

        float currentSize = Geometry.GetBoundsIndependentFromRotation(trans).size.y;

        Vector3 scale = pivot.localScale;

        scale.y = height * scale.y / currentSize;

        pivot.localScale = scale;

        pivot.parent = parent;
    }

    public void Accept()
    {
        if (callWithResult != null)
        {
            callWithResult(text);
        }
        Close();
    }

    public bool Shift
    {
        get { return shift; }
        set { shift = value; UpdateKeys(value); }
    }

    public bool Capslock
    {
        get;
        set;
    }

    private void UpdateKeys(bool shiftOn)
    {
        foreach (Key key in letterKeys)
        {
            key.Shift(shiftOn);
        }
    }
}
