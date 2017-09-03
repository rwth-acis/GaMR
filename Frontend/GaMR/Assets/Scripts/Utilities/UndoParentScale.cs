using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndoParentScale : MonoBehaviour
{

    // Use this for initialization
    void Update()
    {
        transform.localScale = new Vector3(
            1 / transform.parent.localScale.x,
            1 / transform.parent.localScale.y,
            1 / transform.parent.localScale.z
            );

    }
}
