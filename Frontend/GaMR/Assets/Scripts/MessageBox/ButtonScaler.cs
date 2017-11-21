using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ButtonScaler : MonoBehaviour
{
    Transform frameObject;
    SpriteRenderer frame;
    Transform icon;
    Transform captionObject;
    Vector3 originalSize;

    void Awake()
    {
        originalSize = transform.localScale;
        frameObject = transform.Find("Frame");
        if (frameObject != null)
        {
            frame = frameObject.GetComponent<SpriteRenderer>();
        }

        icon = transform.Find("Icon");
        captionObject = transform.Find("Caption");

    }

    private void UndoScaling(Transform parent, Transform child)
    {
        child.localScale = new Vector3(
            child.localScale.x / parent.localScale.x,
            child.localScale.y / parent.localScale.y,
            child.localScale.z / parent.localScale.z);
    }

    void Update()
    {
        //UndoScaling(transform, icon);
    }
}
