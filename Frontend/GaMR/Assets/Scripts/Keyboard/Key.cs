using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// represents one key on a keyboard
/// </summary>
[ExecuteInEditMode]
public class Key : MonoBehaviour, IInputHandler {
    [Tooltip("The type of the key. This determines its functionality if it is pressed")]
    public KeyType keyType = KeyType.LETTER;
    [Tooltip("If keyType is LETTER: the letter which is shown by the key")]
    public string letter;
    private Keyboard keyboard;
    private TextMesh caption;
    private Transform capslockIndication; 

    /// <summary>
    /// get the necessary components:
    /// finds the associated keyboard and the text-object if one is attached
    /// if keyType is CAPSLOCK it also finds the capslock-indication
    /// </summary>
    public void Start()
    {
        keyboard = transform.parent.GetComponent<Keyboard>();
        Transform capObj = transform.Find("Caption");
        if (capObj != null)
        {
            caption = capObj.GetComponent<TextMesh>();
        }

        if (keyType == KeyType.CAPSLOCK)
        {
            capslockIndication = transform.Find("Capslock");
        }
    }

    /// <summary>
    /// Provides convenience in the editor:
    /// if the letter of the key is changed in this script,
    /// it automatically changes the caption on the text object
    /// </summary>
    public void Update()
    {
        // this is called in the editor because of the [ExecuteInEditMode]
        // automatically update the caption with the specified letter
        if (!Application.isPlaying && keyType == KeyType.LETTER)
        {
            caption.text = letter;
            gameObject.name = "Key " + letter;
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
    /// for CANCEL: tell teh keyboard to cancle the input
    /// </summary>
    public void KeyPressed()
    {
        if (keyboard != null)
        {
            if (keyType == KeyType.LETTER)
            {
                if (caption != null)
                {
                    // add the caption to the text
                    keyboard.Text += letter;
                }
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
                capslockIndication.gameObject.SetActive(keyboard.Capslock);
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

    public void OnInputUp(InputEventData eventData)
    {
    }

    /// <summary>
    /// Called if the key is pressed
    /// </summary>
    /// <param name="eventData"></param>
    public void OnInputDown(InputEventData eventData)
    {
        KeyPressed();
        Debug.Log("Key Pressed");
    }

    /// <summary>
    /// method which applies the shift-setting of the keyboard to the letter and to the caption text-mesh
    /// </summary>
    /// <param name="shiftOn"></param>
    public void Shift(bool shiftOn)
    {
        if (shiftOn)
        {
            letter = letter.ToUpper();
        }
        else
        {
            letter = letter.ToLower();
        }
        caption.text = letter;
    }
}

/// <summary>
/// The types which a key can have
/// </summary>
public enum KeyType
{
    LETTER, ACCEPT, CANCEL, BACK, ENTER, SHIFT, CAPSLOCK
}