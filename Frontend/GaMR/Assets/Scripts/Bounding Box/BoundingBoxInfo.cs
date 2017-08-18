using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingBoxInfo : MonoBehaviour
{

    private static int id = 0;
    private int localId;
    private ObjectInfo objectInfo;

    private void Start()
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

}
