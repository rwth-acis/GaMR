using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using HoloToolkit.Sharing.Tests;
using HoloToolkit.Sharing;

public class AnnotationManager : MonoBehaviour
{

    protected List<Annotation> annotations;
    protected List<AnnotationContainer> annotationContainers;
    protected bool editMode = true;
    protected GazeManager gazeManager;
    protected InformationManager infoManager;
    protected ObjectInfo objectInfo;

    protected string subPathLoad = "/resources/annotation/load/";
    protected string subPathSave = "/resources/annotation/save/";

    protected float annotationSize = 7.5f;

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

    private void RemoteAnnotationsUpdated(NetworkInMessage msg)
    {
        Debug.Log("Received remote annotations update");
        if (msg.ReadInt64() != SharingStage.Instance.Manager.GetLocalUser().GetID())
        {
            string modelName = msg.ReadString();
            if (modelName == objectInfo.ModelName)
            {
                LoadAnnotations();
            }
        }
    }

    /// <summary>
    /// initializes and gets the necessary components
    /// </summary>
    protected void Init()
    {
        annotations = new List<Annotation>();
        annotationContainers = new List<AnnotationContainer>();
        gazeManager = ComponentGetter.GetComponentOnGameobject<GazeManager>("InputManager");
        infoManager = ComponentGetter.GetComponentOnGameobject<InformationManager>("InformationManager");
        objectInfo = GetComponent<ObjectInfo>();
    }

    /// <summary>
    /// loads the annotations
    /// </summary>
    protected virtual void LoadAnnotations()
    {
        Debug.Log("Reloading annotations");
        RestManager.Instance.GET(infoManager.FullBackendAddress + subPathLoad, Load, null);
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
            annotationObject.transform.localScale = new Vector3(annotationSize, annotationSize, annotationSize);

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
    public virtual void Add(AnnotationContainer annotationContainer)
    {
        annotations.Add(annotationContainer.Annotation);
        annotationContainers.Add(annotationContainer);
        Save();
    }

    /// <summary>
    /// Deletes an annotation from the list
    /// </summary>
    /// <param name="annotation">The annotation to delete</param>
    public virtual void Delete(AnnotationContainer annotationContainer)
    {
        annotations.Remove(annotationContainer.Annotation);
        annotationContainers.Remove(annotationContainer);
        Save();
    }

    public virtual void NotifyAnnotationEdited(Annotation updatedAnnotation)
    {
        Save();
    }

    /// <summary>
    /// saves all annotations by communicating the list of annotations to the backend
    /// </summary>
    protected virtual void Save(bool synchronize = true)
    {
        JsonAnnotationArray array = new JsonAnnotationArray();
        array.array = annotations;

        string jsonPost = JsonUtility.ToJson(array);
        RestManager.Instance.POST(infoManager.FullBackendAddress + subPathSave, jsonPost,
            (req) =>
            {
                if (synchronize && CustomMessages.Instance != null)
                {
                    CustomMessages.Instance.SendAnnotationsUpdated(objectInfo.ModelName);
                }
            }
            );
    }

    /// <summary>
    /// saves the current set of annotations as a quiz
    /// </summary>
    /// <param name="name">The name of the quiz</param>
    public void SaveAsQuiz(string name)
    {
        string subQuizPathName = "/resources/quiz/save/" + objectInfo.ModelName + "/";

        JsonAnnotationArray array = new JsonAnnotationArray();
        array.array = annotations;

        string jsonPost = JsonUtility.ToJson(array);
        RestManager.Instance.POST(infoManager.FullBackendAddress + subQuizPathName + name, jsonPost);

    }

    /// <summary>
    /// loads the annotations
    /// is called when the rest query has finished
    /// </summary>
    /// <param name="res">The finished web request</param>
    /// <param name="args">Additional arguments which have been passed through the rest-query</param>
    protected void Load(UnityWebRequest res, object[] args)
    {
        if (res.responseCode == 200)
        {
            JsonAnnotationArray array = JsonUtility.FromJson<JsonAnnotationArray>(res.downloadHandler.text);
            annotations = array.array;
            ShowAllAnnotations();
        }
    }

    /// <summary>
    /// hides all annotations
    /// </summary>
    public void HideAllAnnotations()
    {
        foreach (AnnotationContainer container in annotationContainers)
        {
            Destroy(container.gameObject);
        }
        // clear the list
        annotationContainers.Clear();
    }

    /// <summary>
    /// shows all annotations
    /// at first it clears all annotationContainers if some still existed
    /// </summary>
    public void ShowAllAnnotations()
    {
        if (annotationContainers.Count != 0)
        {
            HideAllAnnotations();
        }

        foreach (Annotation annotation in annotations)
        {
            GameObject annotationObject = (GameObject)Instantiate(Resources.Load("AnnotationSphere"));
            annotationObject.transform.parent = gameObject.transform;
            annotationObject.transform.localPosition = annotation.Position;
            annotationObject.transform.localScale = new Vector3(annotationSize, annotationSize, annotationSize);


            AnnotationContainer container = annotationObject.AddComponent<AnnotationContainer>();
            container.annotationManager = this;
            container.loaded = true;
            container.Annotation = annotation;
            annotationContainers.Add(container);
        }
    }

    /// <summary>
    /// whether the edit mode is enabled
    /// </summary>
    public bool EditMode
    {
        get { return editMode; }
        set { editMode = value; }
    }

    /// <summary>
    /// called when the component is destroyed
    /// saves the annotations
    /// </summary>
    public virtual void OnDestroy()
    {
        Save(false);
    }

    /// <summary>
    /// called if the application focus changes
    /// saves the annotations if the application is not focused anymore
    /// </summary>
    /// <param name="focus">true if application is now focused; false if not</param>
    public void OnApplicationFocus(bool focus)
    {
        if (focus == false)
        {
            Save(false);
        }
    }

    /// <summary>
    /// the set of annotations
    /// </summary>
    public List<Annotation> Annotations
    {
        get { return annotations; }
        set { annotations = value; ShowAllAnnotations(); }
    }
}
