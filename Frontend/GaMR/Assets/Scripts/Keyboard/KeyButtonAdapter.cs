using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class KeyButtonAdapter : MonoBehaviour
{

    [Tooltip("The type of the key. This determines its functionality if it is pressed")]
    public KeyType keyType = KeyType.LETTER;
    [Tooltip("If keyType is LETTER: the letter which is shown by the key")]
    [SerializeField]
    private string letter;
    private Keyboard keyboard;
    private FocusableButton focusableButtonComponent;
    private ButtonConfiguration buttonConfiguration;
    private int tryGetComponent = 0;

    void Awake()
    {
        focusableButtonComponent = GetComponent<FocusableButton>();
        if (focusableButtonComponent != null)
        {
            focusableButtonComponent.OnPressed = KeyPressed;
        }

        buttonConfiguration = GetComponent<ButtonConfiguration>();
        keyboard = transform.parent.GetComponent<Keyboard>();

    }

    /// <summary>
    /// Provides convenience in the editor:
    /// if the letter of the key is changed in this script,
    /// it automatically changes the caption on the text object
    /// </summary>
    void Update()
    {
        // this is called in the editor because of the[ExecuteInEditMode]
        // automatically update the button's caption with the specified letter
        if (!Application.isPlaying && keyType == KeyType.LETTER)
        {
            if (buttonConfiguration != null)
            {
                buttonConfiguration.caption = letter;
            }
            else
            {
                if (focusableButtonComponent != null)
                {
                    focusableButtonComponent.Text = letter;
                }
            }
            gameObject.name = "Key " + letter;
        }
    }

    public string Letter
    {
        get { return letter; }
        set
        {
            letter = value;
            if (focusableButtonComponent == null)
            {
                focusableButtonComponent = GetComponent<FocusableButton>();
            }
            if (focusableButtonComponent != null)
            {
                focusableButtonComponent.Text = letter;
            }
        }
    }

    /// <summary>
    /// called if the key is pressed
    /// executes the logical functionality depending on the keyType
    /// for LETTER: add the letter to the keyboard-text
    /// for BACK: remove the last char from the keyboard-text
    /// for ENTER: insert a new line if the maximum of lines is not exceeded
    /// for SHIFT: activate shift
    /// for CAPSLOCK: activate capslock
    /// for ACCEPT: tell the keyboard to finish the input
    /// for CANCEL: tell the keyboard to cancel the input
    /// </summary>
    public void KeyPressed()
    {
        if (keyboard != null)
        {
            if (keyType == KeyType.LETTER)
            {
                // add the letter to the text
                keyboard.Text += letter;
            }
            else if (keyType == KeyType.BACK)
            {
                // remove the last letter from the text
                if (keyboard.Text.Length > 0)
                {
                    keyboard.Text = keyboard.Text.Substring(0, keyboard.Text.Length - 1);
                }
            }
            else if (keyType == KeyType.ENTER)
            {
                keyboard.Text += Environment.NewLine;
            }
            else if (keyType == KeyType.SHIFT)
            {
                if (!keyboard.Capslock)
                {
                    keyboard.Shift = !keyboard.Shift;
                }
            }
            else if (keyType == KeyType.CAPSLOCK)
            {
                keyboard.Capslock = !keyboard.Capslock;
                keyboard.Shift = keyboard.Capslock;
            }
            else if (keyType == KeyType.ACCEPT)
            {
                keyboard.Accept();
            }
            else
            {
                keyboard.Cancel();
            }
        }
    }

    /// <summary>
    /// method which applies the shift-setting of the keyboard to the letter and to the caption text-mesh
    /// </summary>
    /// <param name="shiftOn"></param>
    public void Shift(bool shiftOn)
    {
        if (keyType == KeyType.LETTER)
        {
            // the property Letter automatically handles the changes for the display
            if (shiftOn)
            {
                Letter = letter.ToUpper();
            }
            else
            {
                Letter = letter.ToLower();
            }
        }
    }

}

/// <summary>
/// The types which a key can have
/// </summary>
public enum KeyType
{
    LETTER, ACCEPT, CANCEL, BACK, ENTER, SHIFT, CAPSLOCK
}
