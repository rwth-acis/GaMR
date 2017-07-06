using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizManager : AnnotationManager
{
    protected new string subPathLoad = "/resources/quiz/load/";
    protected new string subPathSave = "/resources/quiz/save/";

    private AnnotationManager annotationManager;

    public string QuizName { get; set; }
    public bool inEditMode { get; set; }

    public new void Start()
    {
        Init();
        subPathLoad += objectInfo.ModelName + "/";
        subPathSave += objectInfo.ModelName + "/";
        // hide the annotations from the standard-annotation manager
        annotationManager = GetComponent<AnnotationManager>();
        annotationManager.HideAllAnnotations();

        if (InformationManager.instance.playerType == PlayerType.AUTHOR)
        {
            inEditMode = true;
        }

        // load the annotations/quiz questions
        LoadAnnotations();
    }

    protected new void Save()
    {
        JsonArray<Annotation> array = new JsonArray<Annotation>();
        array.array = annotations;

        string jsonPost = JsonUtility.ToJson(array);
        if (restManager != null)
        {
            restManager.POST(infoManager.BackendAddress + subPathSave + QuizName, jsonPost);
        }
    }

    protected new void LoadAnnotations()
    {
        restManager.GET(infoManager.BackendAddress + subPathLoad + QuizName, Load);
    }

    public new void OnDestroy()
    {
        Save();
    }

    public new void OnApplicationFocus(bool focus)
    {
        if (focus == false)
        {
            Save();
        }
    }


}
