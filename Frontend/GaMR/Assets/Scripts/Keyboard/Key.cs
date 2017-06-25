using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Key : MonoBehaviour, IInputHandler {

    public KeyType keyType = KeyType.LETTER;
    private Keyboard keyboard;
    private TextMesh caption;


    public void Start()
    {
        keyboard = transform.parent.GetComponent<Keyboard>();
        Transform capObj = transform.Find("Caption");
        if (capObj != null)
        {
            caption = capObj.GetComponent<TextMesh>();
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
                    keyboard.Text += caption.text;
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
}

public enum KeyType
{
    LETTER, ACCEPT, CANCEL, BACK
}