using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class JsonStringItemArray {

    public List<StringItem> strings;

    public JsonStringItemArray()
    {
        strings = new List<StringItem>();
    }

}
