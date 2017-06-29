using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Container class for serializing and de-serializing of arrays
/// This is needed since JSONUtility does not support arrays at the root of JSON
/// </summary>
[Serializable]
public class JSONArray<T>
{
    public List<T> array;
}
