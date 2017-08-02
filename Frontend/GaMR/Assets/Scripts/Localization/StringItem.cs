using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// organizes one key-value pair of two strings
/// used for the serialization of the localization manager dictionaries
/// </summary>
[Serializable]
public class StringItem {

    /// <summary>
    /// The key of the key-value pair
    /// </summary>
    public string key;
    /// <summary>
    /// The value of the key-value pair
    /// </summary>
    public string value;

    /// <summary>
    /// Sets the specifies key and value of the key-value pair
    /// </summary>
    /// <param name="key">The key of the key-value pair</param>
    /// <param name="value">The value of the key-value pair</param>
    public StringItem(string key, string value)
    {
        this.key = key;
        this.value = value;
    }
}
