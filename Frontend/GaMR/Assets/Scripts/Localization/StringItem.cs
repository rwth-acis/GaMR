using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StringItem {

    public string key;
    public string value;

    public StringItem(string key, string value)
    {
        this.key = key;
        this.value = value;
    }
}
