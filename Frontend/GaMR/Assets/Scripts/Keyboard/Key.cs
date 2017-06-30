using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[ExecuteInEditMode]
public class Key : MonoBehaviour, IInputHandler {

    public KeyType keyType = KeyType.LETTER;
    public string letter;
    private Keyboard keyboard;
    private TextMesh caption;
    private Transform capslockIndication; 


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

    public void OnInputDown(InputEventData eventData)
    {
        KeyPressed();
        Debug.Log("Key Pressed");
    }

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

public enum KeyType
{
    LETTER, ACCEPT, CANCEL, BACK, ENTER, SHIFT, CAPSLOCK
}