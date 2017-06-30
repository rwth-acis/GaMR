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
    private RestManager restManager;
    private InformationManager infoManager;

    // Use this for initialization
    void Start()
    {
        annotations = new List<Annotation>();
        gazeManager = ComponentGetter.GetComponentOnGameobject<GazeManager>("InputManager");
        restManager = ComponentGetter.GetComponentOnGameobject<RestManager>("RestManager");
        infoManager = ComponentGetter.GetComponentOnGameobject<InformationManager>("InformationManager");
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
        if (restManager != null)
        {
            restManager.POST(infoManager.BackendAddress + "/resources/annotation/Skull", jsonPost);
        }
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

    public void OnDestroy()
    {
        Save();
    }

    public void OnApplicationFocus(bool focus)
    {
        if (focus == false)
        {
            Save();
        }
    }
}
