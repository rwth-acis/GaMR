using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// list of annotations which is serializable to a JSON array
/// </summary>
[Serializable]
public class JsonAnnotationArray
{
    [SerializeField]
    public List<Annotation> array;
}
