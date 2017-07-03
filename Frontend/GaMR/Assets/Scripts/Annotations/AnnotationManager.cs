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
    private ObjectInfo objectInfo;

    // Use this for initialization
    void Start()
    {
        annotations = new List<Annotation>();
        gazeManager = ComponentGetter.GetComponentOnGameobject<GazeManager>("InputManager");
        restManager = ComponentGetter.GetComponentOnGameobject<RestManager>("RestManager");
        infoManager = ComponentGetter.GetComponentOnGameobject<InformationManager>("InformationManager");
        objectInfo = GetComponent<ObjectInfo>();
    }

    public void TapOnModel()
    {
        if (editMode)
        {
            GameObject annotationObject = (GameObject)Instantiate(Resources.Load("AnnotationSphere"));
            annotationObject.transform.position = gazeManager.HitPosition;
            annotationObject.transform.parent = gameObject.transform;

            // close currently opened annotation box
            if (AnnotationBox.currentlyOpenAnnotationBox != null)
            {
                AnnotationBox.currentlyOpenAnnotationBox.Close();
            }
            // close keyboard if opened
            if (Keyboard.currentlyOpenedKeyboard != null)
            {
                Keyboard.currentlyOpenedKeyboard.Cancel();
            }

            AnnotationContainer container = annotationObject.AddComponent<AnnotationContainer>();
            container.annotationManager = this;
        }
    }

    public void Add(Annotation annotation)
    {
        annotations.Add(annotation);
    }

    public void Delete(Annotation annotation)
    {
        annotations.Remove(annotation);
    }

    public void Save()
    {
        JSONArray<Annotation> array = new JSONArray<Annotation>();
        array.array = annotations;

        string jsonPost = JsonUtility.ToJson(array);
        if (restManager != null)
        {
            restManager.POST(infoManager.BackendAddress + "/resources/annotation/" + objectInfo.ModelName, jsonPost);
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
