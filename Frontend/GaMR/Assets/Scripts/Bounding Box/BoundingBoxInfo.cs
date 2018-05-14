using HoloToolkit.Sharing.Tests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Sharing;
using System;
using HoloToolkit.Unity.InputModule;

public class BoundingBoxInfo : MonoBehaviour
{
    private ObjectInfo objectInfo;

    public BoundingBoxId Id
    {
        get;set;
    }

    public ObjectInfo ObjectInfo
    {
        get
        {
            if (objectInfo == null)
            {
                objectInfo = GetComponentInChildren<ObjectInfo>();
            }
            return objectInfo;
        }
    }



    public BoundingBoxMenu Menu
    {
        get;set;
    }

    public QuizSelectionMenu QuizSelectionMenu
    {
        get;set;
    }
}
