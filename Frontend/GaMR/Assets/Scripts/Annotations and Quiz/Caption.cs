using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles text behavior of a TextMesh which has a textBackground and an overall background
/// </summary>
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

    /// <summary>
    /// Initializes the component
    /// Gets all necessary transforms and calculates the initial sizes of the backgrounds
    /// </summary>
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


    /// <summary>
    /// Updates the sizes of the backgrounds based on the number of lines of the text
    /// </summary>
    private void AdaptSizes()
    {
        float height = Geometry.GetBoundsIndependentFromRotation(textMesh.transform).size.y;

        float heightDifference = height - originalTextHeight;

        ScaleToHeight(textBackgroundPivot, textBackground, originalTextBackgroundHeight + heightDifference);
        ScaleToHeight(backgroundPivot, background, originalBackgroundHeight + heightDifference);
    }

    /// <summary>
    /// Scales a transform to match the specified absolute height
    /// </summary>
    /// <param name="pivot">The scaling pivot</param>
    /// <param name="trans">The transform to scale</param>
    /// <param name="height">The target height</param>
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

    /// <summary>
    /// Adds a scaling pivot at the upper edge of the gameobject
    /// </summary>
    /// <param name="trans">The transform which should get the pivot</param>
    /// <returns>The positioned pivot transform</returns>
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

    /// <summary>
    /// The text which should be displayed by the TextMesh
    /// Automatically handles size changes if set
    /// </summary>
    public string Text
    {
        get { return text; }
        set { text = value; textMesh.text = value; AdaptSizes(); } 
    }
}
