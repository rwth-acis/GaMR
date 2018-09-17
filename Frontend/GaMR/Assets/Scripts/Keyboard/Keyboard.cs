using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// represents and handles the logic of the 3D keyboard
/// </summary>
public class Keyboard : MonoBehaviour, IWindow
{
    // public variables to set in the editor
    [Tooltip("The TextMesh which should show the text which is put in")]
    public TextMesh inputField;
    [Tooltip("The label which can display a message to the user describing what is currently edited")]
    public TextMesh label;
    [Tooltip("The label which displays how long the input is compared to the maximum number of allowed characters")]
    public TextMesh inputLengthLabel;
    [Tooltip("The maximum number of lines that the input may have at maximum")]
    public int maxNumberOfLines = 4;

    // variables related to the logical keyboard-functionality
    private string text = "";
    public Action<string> callWithResult;
    private List<KeyButtonAdapter> letterKeys;
    private KeyButtonAdapter[] allKeys;
    private bool shift = true;
    private bool textInitialization = true;

    private bool cursorOn = false;
    public float cursorTime = 0.5f;
    private float elapsedTimeSinceCursorToggle = 0f;

    // variables concerning the size-height adaption
    private Transform inputBackgroundPivot, inputBackground;
    private int numberOfNewLines = 1, numberOfOldLines = 1;
    private Transform background, backgroundPivot;
    private float lineHeight;
    private BoxCollider coll;
    private float padding = 0.015f;

    // variables concerning the width limit
    private float maxWidth;

    private int maxLength;


    public static Keyboard currentlyOpenedKeyboard;


    public bool IsFullKeyboard { get; set; }

    public int MaxLength
    {
        get
        {
            return maxLength;
        }
        set
        {
            maxLength = value;
            if (maxLength == 0)
            {
                inputLengthLabel.gameObject.SetActive(false);
            }
            else
            {
                inputLengthLabel.gameObject.SetActive(true);
                inputLengthLabel.text = Text.Length + "/" + maxLength;
            }
        }
    }

    /// <summary>
    /// Finds the necessary components: the background of the keyboard, it collider, all keys 
    /// and keys of type LETTER
    /// Also sets the scaling pivots of the background and text-field and scales them to their initial size
    /// </summary>
    public void Awake()
    {
        // get necessary components
        inputBackground = inputField.transform.parent.Find("Background");
        background = transform.Find("Background");
        coll = GetComponent<BoxCollider>();

        letterKeys = new List<KeyButtonAdapter>();

        Capslock = false;

        // if not on XR: disable input so that wasd-keys for navigation do not interfere with keyboard input
        if (!UnityEngine.XR.XRSettings.enabled)
        {
            DesktopCameraControl.Instance.InputEnabled = false;
            // also replace the trackalong with a KeepInFrontOfCamera script
            SimpleTagalong tagalong = gameObject.GetComponent<SimpleTagalong>();
            if (tagalong != null)
            {
                Destroy(tagalong);
            }
            KeepInFrontOfCamera keepInFrontOfCamera = gameObject.AddComponent<KeepInFrontOfCamera>();
            keepInFrontOfCamera.isBeingPlaced = true;
            keepInFrontOfCamera.distanceInFrontOfCamera = 1.5f;
        }

        allKeys = GetComponentsInChildren<KeyButtonAdapter>();

        foreach (KeyButtonAdapter k in allKeys)
        {
            if (k.keyType == KeyType.LETTER)
            {
                letterKeys.Add(k);
            }
        }

        if (IsFullKeyboard)
        {
            // initialize the letter keys with the keyboard layout
            if (letterKeys.Count == LocalizationManager.Instance.KeyboardLayout.Count)
            {
                List<string> letters = LocalizationManager.Instance.KeyboardLayout;
                for (int i = 0; i < letterKeys.Count; i++)
                {
                    letterKeys[i].Letter = letters[i];
                }
            }
            else
            {
                Debug.LogError("Keyboard-layout file has the wrong number of keys: Should be " + letterKeys.Count + " but is " + LocalizationManager.Instance.KeyboardLayout.Count);
            }
        }

        // geometry calcuations:
        // set the scaling pivots for the backgrounds
        if (inputBackground != null)
        {
            inputBackgroundPivot = CreateScalingPivot(inputBackground);
            maxWidth = maxWidth != 0.0f ? maxWidth : Geometry.GetBoundsIndependentFromRotation(inputBackground).size.x;
        }
        if (background != null)
        {
            backgroundPivot = CreateScalingPivot(background);
        }

        // calculate the height of one text line
        lineHeight = Geometry.GetBoundsIndependentFromRotation(inputField.transform).size.y;

        // scale the input background to fit the text
        //ScaleToHeight(inputBackgroundPivot, inputBackground, lineHeight + padding);
        UpdateHeights();
    }

    /// <summary>
    /// Creates a new gameobject which is located at the upper edge of the given transform
    /// This gameobject can then be used to scale the object and keeping the upper edge constant
    /// </summary>
    /// <param name="transform">The transform which gets the scaling pivot</param>
    /// <returns>The pivot on the upper edge of the transform</returns>
    private Transform CreateScalingPivot(Transform transform)
    {
        GameObject pivotPoint = new GameObject("BackgroundScalingPivot");
        pivotPoint.transform.parent = transform.parent;
        float height = Geometry.GetBoundsIndependentFromRotation(transform).size.y;
        pivotPoint.transform.position = transform.position + new Vector3(0, height / 2, 0);
        transform.parent = pivotPoint.transform;
        return pivotPoint.transform;
    }

    /// <summary>
    /// Instantiates and shows a new keyboard
    /// It is initialized with the given parameters
    /// </summary>
    /// <param name="label">The message to show to the user</param>
    /// <param name="callWithResult">The method to call when the input is finished</param>
    /// <param name="fullKeyboard">If true: The full-text keyboard is displayed; if false: only a numeric
    /// version is shown</param>
    public static void Display(string label, Action<string> callWithResult, bool fullKeyboard)
    {
        Display(label, "", callWithResult, fullKeyboard);
    }

    /// <summary>
    /// Instantiates and shows a new keyboard
    /// It is initialized with the given parameters and its textfield is also filled with the predefined text
    /// </summary>
    /// <param name="label">The message to show to the user</param>
    /// <param name="text">The text which is already in the input field</param>
    /// <param name="callWithResult">The method to call when the input is finished</param>
    /// <param name="fullKeyboard">If true: The full-text keyboard is displayed; if false: only a numeric
    /// version is shown</param>
    public static void Display(string label, string text, Action<string> callWithResult, bool fullKeyboard)
    {
        Display(label, text, 0, callWithResult, fullKeyboard);
    }

    public static void Display(string label, string text, int maxNumberOfCharacters, Action<string> callWithResult, bool fullKeyboard)
    {
        Display(label, text, maxNumberOfCharacters, callWithResult, fullKeyboard, false, Vector3.zero, Quaternion.identity);
    }

    public static void Display(string label, string text, Action<string> callWithResult, bool fullKeyboard, Vector3 position, Quaternion rotation)
    {
        Display(label, text, 0, callWithResult, fullKeyboard, true, position, rotation);
    }

    public static void Display(string label, string text, int maxNumberOfCharacters, Action<string> callWithResult, bool fullKeyboard, Vector3 position, Quaternion rotation)
    {
        Display(label, text, maxNumberOfCharacters, callWithResult, fullKeyboard, true, position, rotation);
    }

    private static void Display(string label, string text, int maxNumberOfCharacters, Action<string> callWithResult, bool fullKeyboard, bool fixedPosition, Vector3 position, Quaternion rotation)
    {
        GameObject keyboardInstance;
        if (fullKeyboard)
        {
            keyboardInstance = GameObject.Instantiate(WindowResources.Instance.Keyboard);
        }
        else
        {
            keyboardInstance = GameObject.Instantiate(WindowResources.Instance.Numpad);
        }
        Keyboard keyboard = keyboardInstance.GetComponent<Keyboard>();
        keyboard.label.text = label;
        keyboard.Text = text;
        keyboard.callWithResult = callWithResult;
        keyboard.IsFullKeyboard = fullKeyboard;
        keyboard.MaxLength = maxNumberOfCharacters;

        if (fixedPosition) // use a fixed position for the keyboardi instead of tracking it with the view
        {
            // delete the components from the prefab which realize the view tracking
            SimpleTagalong tagalong = keyboardInstance.GetComponent<SimpleTagalong>();
            Destroy(tagalong);
            FaceCamera faceCamera = keyboardInstance.GetComponent<FaceCamera>();
            Destroy(faceCamera);
            Window3D window = keyboardInstance.GetComponent<Window3D>();
            Destroy(window);

            // set the position of the keyboard
            keyboardInstance.transform.position = position;
            keyboardInstance.transform.rotation = rotation;
        }

        currentlyOpenedKeyboard = keyboard;
    }

    /// <summary>
    /// The text which is shown in the input-field of the keyboard
    /// Automatically adapts the input-TextMesh to show this text
    /// Also handles size-changes to fit the number of lines
    /// </summary>
    public string Text
    {
        get
        {
            return text;
        }
        set
        {
            if (MaxLength > 0)
            {
                if (value.Length > MaxLength)
                {
                    return;
                }

                inputLengthLabel.text = value.Length + "/" + MaxLength;
            }
            if (inputBackground != null)
            {
                maxWidth = maxWidth != 0.0f ? maxWidth : Geometry.GetBoundsIndependentFromRotation(inputBackground).size.x;
            }
            string wrappedText = AutoLineBreak.StringWithLineBreaks(inputField, value, maxWidth);
            numberOfNewLines = wrappedText.Split('\n').Length;
            if (numberOfNewLines <= maxNumberOfLines)
            {
                text = value; // give the original value to the text
                // update the input field
                UpdateInputField(wrappedText); // but display the wrapped text version
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

    /// <summary>
    /// Applies the text of the keyboard to the input-field
    /// </summary>
    private void UpdateInputField(string newText)
    {
        inputField.text = newText;
        cursorOn = false;
    }

    /// <summary>
    /// Called if the user cancles the input
    /// </summary>
    public void Cancel()
    {
        if (callWithResult != null)
        {
            callWithResult(null);
        }
        Close();
    }

    /// <summary>
    /// Closes the keyboard and clears the current instance
    /// </summary>
    private void Close()
    {
        currentlyOpenedKeyboard = null;
        // if not on XR: re-enable input
        if (!UnityEngine.XR.XRSettings.enabled)
        {
            DesktopCameraControl.Instance.InputEnabled = true;
        }
        Destroy(gameObject);
    }

    /// <summary>
    /// Adapts the heights of the background and the input field to fit the number of lines of the input
    /// Also updates the positions of the keys accordingly
    /// </summary>
    private void UpdateHeights()
    {
        Quaternion rot = transform.rotation;
        transform.rotation = Quaternion.identity;
        int dirFac;
        dirFac = Math.Sign(numberOfNewLines - numberOfOldLines);
        float newTextHeight = Geometry.GetBoundsIndependentFromRotation(inputField.transform).size.y;
        if (inputBackground != null)
        {
            ScaleToHeight(inputBackgroundPivot, inputBackground, newTextHeight + padding);
        }
        foreach (KeyButtonAdapter k in allKeys)
        {
            k.transform.position -= dirFac * new Vector3(0, lineHeight, 0);
        }

        if (background != null)
        {
            float backgroundHeight = Geometry.GetBoundsIndependentFromRotation(background).size.y;
            ScaleToHeight(backgroundPivot, background, backgroundHeight + dirFac * lineHeight);
            coll.size = new Vector3(
                coll.size.x,
                backgroundPivot.localScale.y * background.localScale.y,
                coll.size.z);
            coll.center = background.position - transform.position;
        }

        transform.rotation = rot;
    }

    /// <summary>
    /// Scales a tranform to the specified absolute height
    /// </summary>
    /// <param name="pivot">The scaling pivot of the transform</param>
    /// <param name="trans">The transform to scale</param>
    /// <param name="height">The target height</param>
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

    /// <summary>
    /// Called if the user ends the input and accepts it
    /// </summary>
    public void Accept()
    {
        if (callWithResult != null)
        {
            callWithResult(text);
        }
        Close();
    }

    /// <summary>
    /// The shift-state of the keyboard
    /// When set it automatically updates the key-captions
    /// </summary>
    public bool Shift
    {
        get { return shift; }
        set { shift = value; UpdateKeys(value); }
    }

    /// <summary>
    /// Whether or not capslock is activated
    /// </summary>
    public bool Capslock
    {
        get;
        set;
    }

    /// <summary>
    /// Tells the keys to update their caption to show upper letters when shift is activated
    /// </summary>
    /// <param name="shiftOn"></param>
    private void UpdateKeys(bool shiftOn)
    {
        foreach (KeyButtonAdapter key in letterKeys)
        {
            key.Shift(shiftOn);
        }
    }

    private void Update()
    {
        elapsedTimeSinceCursorToggle += Time.deltaTime;
        if (elapsedTimeSinceCursorToggle >= cursorTime)
        {
            if (cursorOn)
            {
                inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
                cursorOn = false;
            }
            else
            {
                inputField.text += "|";
                cursorOn = true;
            }
            elapsedTimeSinceCursorToggle = 0;
        }
    }

    public void CloseWindow()
    {
        Cancel();
    }
}
