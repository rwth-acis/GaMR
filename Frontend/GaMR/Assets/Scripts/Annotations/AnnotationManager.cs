using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AnnotationManager : MonoBehaviour
{

    protected List<Annotation> annotations;
    protected List<AnnotationContainer> annotationContainers;
    protected bool editMode = true;
    protected GazeManager gazeManager;
    protected RestManager restManager;
    protected InformationManager infoManager;
    protected ObjectInfo objectInfo;

    protected string subPathLoad = "/resources/annotation/load/";
    protected string subPathSave = "/resources/annotation/save/";

    /// <summary>
    /// Initializes the annotation-manager
    /// Collects the necessary components and loads previously stored annotations if they exist
    /// </summary>
    public void Start()
    {
        Init();
        subPathLoad += objectInfo.ModelName;
        subPathSave += objectInfo.ModelName;
        LoadAnnotations();
    }

    protected void Init()
    {
        annotations = new List<Annotation>();
        annotationContainers = new List<AnnotationContainer>();
        gazeManager = ComponentGetter.GetComponentOnGameobject<GazeManager>("InputManager");
        restManager = ComponentGetter.GetComponentOnGameobject<RestManager>("RestManager");
        infoManager = ComponentGetter.GetComponentOnGameobject<InformationManager>("InformationManager");
        objectInfo = GetComponent<ObjectInfo>();
    }

    protected void LoadAnnotations()
    {
        restManager.GET(infoManager.BackendAddress + subPathLoad, Load);
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
    public void Add(AnnotationContainer annotationContainer)
    {
        annotations.Add(annotationContainer.Annotation);
        annotationContainers.Add(annotationContainer);
    }

    /// <summary>
    /// Deletes an annotation from the list
    /// </summary>
    /// <param name="annotation">The annotation to delete</param>
    public void Delete(AnnotationContainer annotationContainer)
    {
        annotations.Remove(annotationContainer.Annotation);
        annotationContainers.Remove(annotationContainer);
    }

    /// <summary>
    /// saves all annotations by communicating the list of annotations to the backend
    /// </summary>
    protected void Save()
    {
        JsonAnnotationArray array = new JsonAnnotationArray();
        array.array = annotations;

        string jsonPost = JsonUtility.ToJson(array);
        if (restManager != null)
        {
            restManager.POST(infoManager.BackendAddress + subPathSave, jsonPost);
        }
    }

    protected void Load(string res)
    {
        if (res != null)
        {
            JsonAnnotationArray array = JsonUtility.FromJson<JsonAnnotationArray>(res);
            annotations = array.array;
            ShowAllAnnotations();
        }
    }

    public void HideAllAnnotations()
    {
        foreach(AnnotationContainer container in annotationContainers)
        {
            Destroy(container.gameObject);
        }
        // clear the list
        annotationContainers.Clear();
        this.enabled = false;
    }

    public void ShowAllAnnotations()
    {
        this.enabled = true;
        if (annotationContainers.Count != 0)
        {
            HideAllAnnotations();
        }

        foreach (Annotation annotation in annotations)
        {
            GameObject annotationObject = (GameObject)Instantiate(Resources.Load("AnnotationSphere"));
            annotationObject.transform.parent = gameObject.transform;
            annotationObject.transform.localPosition = annotation.Position;

            AnnotationContainer container = annotationObject.AddComponent<AnnotationContainer>();
            container.annotationManager = this;
            container.loaded = true;
            container.Annotation = annotation;
            annotationContainers.Add(container);
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
