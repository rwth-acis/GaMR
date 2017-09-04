using HoloToolkit.Sharing.Tests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Sharing;
using System;

public class BoundingBoxInfo : MonoBehaviour
{

    private static int id = 0;
    private int localId;
    private ObjectInfo objectInfo;

    private void Awake()
    {
        localId = id;
        id++;
    }

    public int BoxId
    {
        get { return localId; }
    }

    public ObjectInfo ObjectInfo
    {
        get
        {
            if (objectInfo == null)
            {
                objectInfo = GetComponentInChildren<ObjectInfo>();
            }
            return ObjectInfo;
        }
    }

    private void OnDestroy()
    {
        CustomMessages.Instance.SendModelDelete(localId);
    }
}
