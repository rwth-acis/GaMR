using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using HoloToolkit.Sharing.Tests;
using HoloToolkit.Sharing;

/// <summary>
/// administers the annotations on a model
/// </summary>
public class AnnotationManager : MonoBehaviour
{
    protected List<Annotation> annotations;
    protected List<AnnotationContainer> annotationContainers;
    protected bool editMode = true;
    protected ObjectInfo objectInfo;

    protected string subPathLoad = "/resources/annotation/load/";
    protected string subPathSave = "/resources/annotation/save/";

    private bool initializingAudio = true;

    protected float annotationSize = 7.5f;

    private AnnotationMenu currentlyOpenAnnotationMenu;

    /// <summary>
    /// Gets or sets the annotation menu which is currently open
    /// If set, it automatically closes any other open annotation menu (on the same model)
    /// </summary>
    public AnnotationMenu CurrentlyOpenAnnotationMenu
    {
        get
        {
            return currentlyOpenAnnotationMenu;
        }
        set
        {
            if (currentlyOpenAnnotationMenu != null)
            {
                currentlyOpenAnnotationMenu.Close();
            }
            currentlyOpenAnnotationMenu = value;
        }
    }

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

    /// <summary>
    /// Called if the annotations have been updated by another participant of the shared session
    /// Reloads the annotations
    /// </summary>
    /// <param name="msg">The network message containing the ID of the sender and the model name</param>
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
        objectInfo = GetComponent<ObjectInfo>();
    }

    /// <summary>
    /// loads the annotations
    /// </summary>
    protected virtual void LoadAnnotations()
    {
        Debug.Log("Reloading annotations");
        RestManager.Instance.GET(InformationManager.Instance.FullBackendAddress + subPathLoad, Load, null);
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
            annotationObject.transform.position = GazeManager.Instance.HitPosition;
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
        DeleteAudioAnnotation(annotationContainer);
        annotations.Remove(annotationContainer.Annotation);
        annotationContainers.Remove(annotationContainer);
        Save();
    }

    /// <summary>
    /// Notifies the annotation manager about changes to the annotations
    /// Saves the current annotations
    /// </summary>
    /// <param name="updatedAnnotation"></param>
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
        RestManager.Instance.POST(InformationManager.Instance.FullBackendAddress + subPathSave, jsonPost,
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
    /// saves an audio annotation
    /// should be called immediately after an audio annotation was changed
    /// </summary>
    /// <param name="container">The container which stores the audio annotation</param>
    public void SaveAudioAnnotation(AnnotationContainer container)
    {
        if (!initializingAudio)
        {
            if (container.AnnotationClip != null)
            {
                Debug.Log("Saving clip for " + container.Annotation.PositionToStringWithoutDots + " (" + container.Annotation.Text + ")");
                RestManager.Instance.SendAudioClip(InformationManager.Instance.FullBackendAddress + "/resources/annotation/audio/" + objectInfo.ModelName + "/" + container.Annotation.PositionToStringWithoutDots, container.AnnotationClip, null);
            }
        }
    }

    /// <summary>
    /// Deletes an audio annotation on the backend's storage and destroys the refernce to it in the annotation container
    /// </summary>
    /// <param name="container">The container which holds the audio annotation</param>
    public void DeleteAudioAnnotation(AnnotationContainer container)
    {
        container.AnnotationClip = null;
        RestManager.Instance.DELETE(InformationManager.Instance.FullBackendAddress + "/resources/annotation/audio/" + objectInfo.ModelName + "/" + container.Annotation.PositionToStringWithoutDots, null);
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
        RestManager.Instance.POST(InformationManager.Instance.FullBackendAddress + subQuizPathName + name, jsonPost);

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

        if (annotationContainers.Count > 0)
        {
            LoadAudioAnnotations(0);
        }
    }

    /// <summary>
    /// Loads an audio clip which is stored as an audio annotations from the backend
    /// </summary>
    /// <param name="index">The index of the requested clip</param>
    private void LoadAudioAnnotations(int index)
    {
        Debug.Log("Loading audio for " + index);
        RestManager.Instance.GetAudioClip(InformationManager.Instance.FullBackendAddress + "/resources/annotation/audio/" + objectInfo.ModelName + "/" + annotationContainers[index].Annotation.PositionToStringWithoutDots,
            (clip, resCode) =>
            {
                if (resCode == 200)
                {
                    annotationContainers[index].AnnotationClip = clip;
                }

                // to avoid placing all requests at once and stressing the server, the request for the next
                // index is only executed once the previous query has finished
                if (index < annotationContainers.Count - 1)
                {
                    LoadAudioAnnotations(index + 1);
                }
                else
                {
                    // no more audio files to load => indicate this in a Boolean
                    initializingAudio = false;
                }
            });
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
        if (currentlyOpenAnnotationMenu != null)
        {
            currentlyOpenAnnotationMenu.Close();
        }
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
