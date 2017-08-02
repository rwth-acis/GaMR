using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component which enables bluetooth input for a keyboard on the same gameobject
/// </summary>
[RequireComponent(typeof(Keyboard))]
public class BluetoothKeyboard : MonoBehaviour
{
    private Keyboard keyBoard;
    private GameObject pseudoKeyObject;
    private Key pseudoKey;
    [Tooltip("If true, only the characters specified in White List Chars will be used")]
    public bool useFilter = false;
    [Tooltip("Specify allowed characters without any separators")]
    public string whiteListChars = "";

    /// <summary>
    /// initializes and gets all necessary components
    /// </summary>
    void Start()
    {
        keyBoard = gameObject.GetComponent<Keyboard>();
        pseudoKeyObject = new GameObject("PseudoBluetoothKey");
        pseudoKeyObject.transform.parent = keyBoard.transform;
        pseudoKey = pseudoKeyObject.AddComponent<Key>();
    }

    /// <summary>
    /// recognizes input on the bluetooth keyboard and converts it to input on the 3D keyboard
    /// </summary>
    void Update()
    {
        // any key is held down
        if (Input.anyKey)
        {
            string clearedInputString = "";
            // evaluate the input
            foreach(char c in Input.inputString)
            {
                if (c=='\n' || c=='\r')
                {
                    CommitText(clearedInputString);
                    clearedInputString = "";
                    pseudoKey.keyType = KeyType.ENTER;
                    pseudoKey.KeyPressed();

                }
                else if (c == '\b')
                {
                    CommitText(clearedInputString);
                    clearedInputString = "";
                    pseudoKey.keyType = KeyType.BACK;
                    pseudoKey.KeyPressed();
                }
                else
                {
                    if (useFilter)
                    {
                        // go through the filter and add the char to the clearedInput if it is allowed
                        // if the letter is not allowed it is not added
                        foreach (char allowedChar in whiteListChars)
                        {
                            if (allowedChar == c)
                            {
                                clearedInputString += c.ToString();
                                break; // no need to resume since the char was found
                            }
                        }
                    }
                    else
                    {
                        clearedInputString += c.ToString();
                    }
                }
            }



            CommitText(clearedInputString);
        }
    }

    /// <summary>
    /// commits the given text to the 3D keyboard
    /// </summary>
    /// <param name="input">The text to send to the 3D keyboard</param>
    private void CommitText(string input)
    {
        pseudoKey.keyType = KeyType.LETTER;
        pseudoKey.Letter = input;
        pseudoKey.KeyPressed();
    }
}
