using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Extension of an UnityEvent
/// Listeners take a string as an argument
/// </summary>
[Serializable]
public class StringEvent : UnityEvent<string> { }