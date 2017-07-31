using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Use this for initialization
    void Start()
    {
        keyBoard = gameObject.GetComponent<Keyboard>();
        pseudoKeyObject = new GameObject("PseudoBluetoothKey");
        pseudoKeyObject.transform.parent = keyBoard.transform;
        pseudoKey = pseudoKeyObject.AddComponent<Key>();
    }

    // Update is called once per frame
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

    private void CommitText(string input)
    {
        pseudoKey.keyType = KeyType.LETTER;
        pseudoKey.Letter = input;
        pseudoKey.KeyPressed();
    }
}
