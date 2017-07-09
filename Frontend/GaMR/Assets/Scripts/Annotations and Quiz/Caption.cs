using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caption : MonoBehaviour {

    private Transform textBackgroundPivot, textBackground;
    private Transform background, backgroundPivot;
    private TextMesh textMesh;
    private float originalTextHeight, originalTextBackgroundHeight, originalBackgroundHeight;
    private string text;

    public void Start()
    {
        // not necessary, component is initialized by another component using Init()
    }

    public void Init()
    {
        background = transform.parent.Find("Background");
        textBackground = transform.Find("Background");
        textMesh = transform.Find("Caption").GetComponent<TextMesh>();

        textBackgroundPivot = CreateScalingPivot(textBackground);
        backgroundPivot = CreateScalingPivot(background);

        string origText = textMesh.text;
        textMesh.text = "";
        originalTextHeight = Geometry.GetBoundsIndependentFromRotation(textMesh.transform).size.y;
        textMesh.text = origText;

        originalTextBackgroundHeight = Geometry.GetBoundsIndependentFromRotation(textBackground).size.y;
        originalBackgroundHeight = Geometry.GetBoundsIndependentFromRotation(background).size.y;
    }

    private void AdaptSizes()
    {
        float height = Geometry.GetBoundsIndependentFromRotation(textMesh.transform).size.y;

        float heightDifference = height - originalTextHeight;

        ScaleToHeight(textBackgroundPivot, textBackground, originalTextBackgroundHeight + heightDifference);
        ScaleToHeight(backgroundPivot, background, originalBackgroundHeight + heightDifference);
    }

    private void ScaleToHeight(Transform pivot, Transform trans, float height)
    {
        Transform parent = pivot.parent;
        pivot.parent = null;

        float currentSize = Geometry.GetBoundsIndependentFromRotation(trans).size.y;

        Vector3 scale = pivot.localScale;

        scale.y = height * scale.y / currentSize;

        pivot.localScale = scale;

        pivot.parent = parent;
    }

    private Transform CreateScalingPivot(Transform trans)
    {
        GameObject pivotPoint = new GameObject("BackgroundScalingPivot");
        if (trans.parent != null)
        {
            pivotPoint.transform.parent = trans.parent;
        }
        float height = Geometry.GetBoundsIndependentFromRotation(trans).size.y;
        pivotPoint.transform.position = trans.position + new Vector3(0, height / 2, 0);
        trans.parent = pivotPoint.transform;
        return pivotPoint.transform;
    }

    public string Text
    {
        get { return text; }
        set { text = value; textMesh.text = value; AdaptSizes(); } 
    }
}
