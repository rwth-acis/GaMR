using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ButtonScaler : MonoBehaviour
{
#if UNITY_EDITOR
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

    private void UndoScaling(Transform parent, Transform child, float size)
    {
        Vector3 newScale = new Vector3(
            size / parent.localScale.x,
            size / parent.localScale.y,
            size / parent.localScale.z
            );

        newScale = child.rotation * newScale;
        child.localScale = newScale;
        
    }

    void Update()
    {
        UndoScaling(transform, icon, 0.03f);
    }
#endif
}
