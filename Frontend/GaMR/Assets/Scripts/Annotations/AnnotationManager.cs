using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AnnotationManager : MonoBehaviour
{

    protected List<Annotation> annotations;
    protected bool editMode = true;
    protected GazeManager gazeManager;
    protected RestManager restManager;
    protected InformationManager infoManager;
    protected ObjectInfo objectInfo;

    /// <summary>
    /// Initializes the annotation-manager
    /// Collects the necessary components and loads previously stored annotations if they exist
    /// </summary>
    protected void Start()
    {
        annotations = new List<Annotation>();
        gazeManager = ComponentGetter.GetComponentOnGameobject<GazeManager>("InputManager");
        restManager = ComponentGetter.GetComponentOnGameobject<RestManager>("RestManager");
        infoManager = ComponentGetter.GetComponentOnGameobject<InformationManager>("InformationManager");
        objectInfo = GetComponent<ObjectInfo>();

        LoadAnnotations();
    }

    protected void LoadAnnotations()
    {
        restManager.GET(infoManager.BackendAddress + "/resources/annotation/load/" + objectInfo.ModelName, Load);
    }

    /// <summary>
    /// called if the annotation-object is tapped
    /// shows an annotation box with the corresponding annotation's content
    /// </summary>
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
            container.loaded = false;
            container.annotationManager = this;
        }
    }

    /// <summary>
    /// adds a new annotation to the list
    /// </summary>
    /// <param name="annotation">The annotation to add</param>
    public void Add(Annotation annotation)
    {
        annotations.Add(annotation);
    }

    /// <summary>
    /// Deletes an annotation from the list
    /// </summary>
    /// <param name="annotation">The annotation to delete</param>
    public void Delete(Annotation annotation)
    {
        annotations.Remove(annotation);
    }

    /// <summary>
    /// saves all annotations by communicating the list of annotations to the backend
    /// </summary>
    protected void Save()
    {
        JSONArray<Annotation> array = new JSONArray<Annotation>();
        array.array = annotations;

        string jsonPost = JsonUtility.ToJson(array);
        if (restManager != null)
        {
            restManager.POST(infoManager.BackendAddress + "/resources/annotation/save/" + objectInfo.ModelName, jsonPost);
        }
    }

    protected void Load(string res)
    {
        if (res != null)
        {
            JSONArray<Annotation> array = JsonUtility.FromJson<JSONArray<Annotation>>(res);
            foreach(Annotation annotation in array.array)
            {
                GameObject annotationObject = (GameObject)Instantiate(Resources.Load("AnnotationSphere"));
                annotationObject.transform.localPosition = annotation.Position;
                annotationObject.transform.parent = gameObject.transform;

                AnnotationContainer container = annotationObject.AddComponent<AnnotationContainer>();
                container.annotationManager = this;
                container.loaded = true;
                container.Annotation = annotation;
            }
            annotations = array.array;
        }
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
