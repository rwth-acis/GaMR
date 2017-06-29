using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AnnotationManager : MonoBehaviour
{

    private List<Annotation> annotations;
    private bool editMode = true;
    private GazeManager gazeManager;

    // Use this for initialization
    void Start()
    {
        annotations = new List<Annotation>();
        gazeManager = ComponentGetter.GetComponentOnGameobject<GazeManager>("InputManager");
    }

    public void TapOnModel()
    {
        if (editMode)
        {
            GameObject annotationObject = (GameObject)Instantiate(Resources.Load("AnnotationSphere"));
            annotationObject.transform.position = gazeManager.HitPosition;
            annotationObject.transform.parent = gameObject.transform;
            Add(annotationObject.transform.localPosition, "");
        }
    }

    private void Add(Vector3 position, string text)
    {
        annotations.Add(new Annotation(position, text));
    }

    public void Save()
    {
        JSONArray<Annotation> array = new JSONArray<Annotation>();
        array.array = annotations;

        string jsonPost = JsonUtility.ToJson(array);
    }

    public void Load(string res)
    {
        JSONArray<Annotation> array = JsonUtility.FromJson<JSONArray<Annotation>>(res);
    }

    public bool EditMode
    {
        get { return editMode; }
        set { editMode = value; }
    }
}
