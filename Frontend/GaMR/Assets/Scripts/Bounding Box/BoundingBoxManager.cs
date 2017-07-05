using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles scaling for the wires of the bounding box
/// keeps the the wires circular, even if the bounding box is scaled
/// also keeps the transformation widgets at their absolute size
/// </summary>
public class BoundingBoxManager : MonoBehaviour
{

    [Tooltip("Diameter/Thickness of the wires which form the bounding-box")]
    public float wireDiameter = 0.01f;

    [Tooltip("Factor for the absolute size of the transformation widgets")]
    public float widgetSize = 0.03f;

    public float menuSize = 1f;

    private Transform xAxis, yAxis, zAxis, menu;
    private List<Transform> widgets;

    /// <summary>
    /// gets the necessary comoponents: children for menus and all widgets
    /// </summary>
    void Start()
    {
        // get the axes of the bounding-box
        xAxis = gameObject.transform.Find("X");
        yAxis = gameObject.transform.Find("Y");
        zAxis = gameObject.transform.Find("Z");
        menu = gameObject.transform.Find("MenuCenter");

        // get all widgets
        widgets = new List<Transform>();

        foreach(Transform t in gameObject.transform.Find("Widgets/Scale"))
        {
            widgets.Add(t);
        }
        foreach(Transform t in gameObject.transform.Find("Widgets/Rotation"))
        {
            widgets.Add(t);
        }
    }

    /// <summary>
    /// Keeps the handles at the same size and undoes stretching to the wires
    /// </summary>
    void Update()
    {
        CompensateAxisDeformation();
        foreach (Transform trans in widgets)
        {
            CompensateParentScale(trans, widgetSize);
        }
        CompensateParentScale(menu, menuSize);
    }

    /// <summary>
    /// Compensates the scaling of the parent in order to maintain circular wires
    /// this means that only two axes need to be corrected
    /// </summary>
    private void CompensateAxisDeformation()
    {
        float x = wireDiameter / transform.localScale.x;
        float y = wireDiameter / transform.localScale.y;
        float z = wireDiameter / transform.localScale.z;

        // y-scale of 0.5f since the cube initially has a size of 1

        foreach (Transform trans in xAxis.transform)
        {
            trans.localScale = new Vector3(y, 0.5f, z); // changed order since they are rotated
        }

        foreach (Transform trans in yAxis.transform)
        {
            trans.localScale = new Vector3(x, 0.5f, z);
        }

        foreach (Transform trans in zAxis.transform)
        {
            trans.localScale = new Vector3(x, 0.5f, y); // changed order since they are rotated
        }

    }

    /// <summary>
    /// Compensates size changes so that the object 
    /// </summary>
    private void CompensateParentScale(Transform toCorrect, float size)
    {
        // keep widgets at the same size

        float x = size / transform.localScale.x;
        float y = size / transform.localScale.y;
        float z = size / transform.localScale.z;

        toCorrect.localScale = new Vector3(x, y, z);
    }
}
