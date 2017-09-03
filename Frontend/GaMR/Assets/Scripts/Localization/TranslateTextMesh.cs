using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslateTextMesh : MonoBehaviour
{
    TextMesh textMesh;

    // Use this for initialization
    void Start()
    {
        textMesh = GetComponent<TextMesh>();
        textMesh.text = LocalizationManager.Instance.ResolveString(textMesh.text);
    }
}
