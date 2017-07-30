using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Keyboard))]
public class BluetoothKeyboard : MonoBehaviour
{
    private Keyboard keyBoard;
    private GameObject pseudoKeyObject;
    private Key pseudoKey;

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
                    pseudoKey.Letter = Input.inputString;
                }

                pseudoKey.KeyPressed();
            }
        }
    }
}
