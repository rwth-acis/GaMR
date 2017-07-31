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
            // evaluate the input
            foreach(char c in Input.inputString)
            {
                if (c=='\n' || c=='\r')
                {
                    pseudoKey.keyType = KeyType.ENTER;

                }
                else if (c == '\b')
                {
                    pseudoKey.keyType = KeyType.BACK;
                }
                else
                {
                    pseudoKey.keyType = KeyType.LETTER;
                    if (useFilter)
                    {
                        pseudoKey.Letter = "";
                        // go through the filter and assign the char if it is allowed
                        // if the letter is not allowed it is not set and the key will have "" as a letter
                        foreach(char allowedChar in whiteListChars)
                        {
                            if (allowedChar == c)
                            {
                                pseudoKey.Letter = c.ToString();
                                break; // no need to resume since the char was found
                            }
                        }
                    }
                    else // just assign the letter
                    {
                        pseudoKey.Letter = c.ToString();
                    }
                }

                pseudoKey.KeyPressed();
            }
        }
    }
}
