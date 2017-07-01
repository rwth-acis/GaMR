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
    private GameObject currentlyEditedAnnotation;

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

            AnnotationContainer container = annotationObject.AddComponent<AnnotationContainer>();
            container.annotationManager = this;

            //currentlyEditedAnnotation = annotationObject;
            //Keyboard.Display("Enter the text of the annotation", UserInputFinished, true);
        }
    }

    //private void UserInputFinished(string input)
    //{
    //    if (input == null) // user canceled
    //    {
    //        Destroy(currentlyEditedAnnotation);
    //    }
    //    else
    //    {
    //        Add(currentlyEditedAnnotation.transform.localPosition, input);
    //    }
    //}

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
