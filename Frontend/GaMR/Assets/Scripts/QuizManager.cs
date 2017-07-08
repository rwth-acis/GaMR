using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizManager : AnnotationManager
{
    protected new string subPathLoad = "/resources/quiz/load/";
    protected new string subPathSave = "/resources/quiz/save/";

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
        restManager.GET(infoManager.BackendAddress + subPathLoad + QuizName, QuizLoaded);
    }

    private void QuizLoaded(string res)
    {
        Load(res);
        InitializeQuiz();
    }

    private void InitializeQuiz()
    {
        int value = (int) UnityEngine.Random.Range(0, 2);
        if (value == 0)
        {
            PositionToName = true;
            MessageBox.Show("Click on the annotations\n and enter the name of the\n corresponding part",MessageBoxType.INFORMATION);
        }
        else
        {
            PositionToName = false;
            MessageBox.Show("Connect the names on the right\n with their corresponding position", MessageBoxType.INFORMATION);
        }
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
