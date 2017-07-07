using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizManager : AnnotationManager
{
    protected new string subPathLoad = "/resources/quiz/load/";
    protected new string subPathSave = "/resources/quiz/save/";

    private AnnotationManager annotationManager;
    private Annotation currentQuestion;

    public string QuizName { get; set; }

    public bool PositionToName { get; set; }

    public new void Start()
    {
        Init();
        subPathLoad += objectInfo.ModelName + "/";
        subPathSave += objectInfo.ModelName + "/";

        if (InformationManager.instance.playerType == PlayerType.STUDENT)
        {
            editMode = false;
        }
        else
        {
            editMode = true;
        }

        // load the annotations/quiz questions
        LoadAnnotations();
    }

    protected override void Save()
    {
        JsonArray<Annotation> array = new JsonArray<Annotation>();
        array.array = annotations;

        string jsonPost = JsonUtility.ToJson(array);
        if (restManager != null)
        {
            restManager.POST(infoManager.BackendAddress + subPathSave + QuizName, jsonPost);
        }
    }

    protected override void LoadAnnotations()
    {
        restManager.GET(infoManager.BackendAddress + subPathLoad + QuizName, Load);
    }


    public void EvaluateQuestion(Annotation annotation)
    {
        EvaluateQuestion(annotation, currentQuestion.Text);
    }

    public void EvaluateQuestion(Annotation annotation, string input)
    {
        if (annotation.Text == input)
        {
            MessageBox.Show("Correct", MessageBoxType.SUCCESS);
        }
        else
        {
            MessageBox.Show("Incorrect", MessageBoxType.ERROR);
        }
    }
}
