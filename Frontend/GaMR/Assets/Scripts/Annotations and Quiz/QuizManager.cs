using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizManager : AnnotationManager
{
    protected new string subPathLoad = "/resources/quiz/load/";
    protected new string subPathSave = "/resources/quiz/save/";

    private string currentQuestion;
    private ObjectInfo objInfo;
    private GameObject quizObject;

    public string QuizName { get; set; }

    public bool PositionToName { get; set; }

    public new void Start()
    {
        Init();
        objInfo = GetComponent<ObjectInfo>();
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
        if (InformationManager.instance.playerType != PlayerType.STUDENT)
        {
            // if not student => save the quiz itself
            JsonArray<Annotation> array = new JsonArray<Annotation>();
            array.array = annotations;

            string jsonPost = JsonUtility.ToJson(array);
            if (restManager != null)
            {
                restManager.POST(infoManager.BackendAddress + subPathSave + QuizName, jsonPost);
            }
        }
        else // if it is a student => save the achievements
        {

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
        int value = UnityEngine.Random.Range(0, 2);
        if (value == 0)
        {
            PositionToName = true;
            MessageBox.Show("Click on the annotations\n and enter the name of the\n corresponding part",MessageBoxType.INFORMATION);
        }
        else
        {
            PositionToName = false;
            MessageBox.Show("Connect the names\n with their corresponding position", MessageBoxType.INFORMATION);
            quizObject = new GameObject("Quiz");
            quizObject.transform.parent = gameObject.transform.parent.parent; // the bounding box is the parent
            quizObject.transform.position = gameObject.transform.position + new Vector3(objInfo.Size.x, 0, 0);

            Menu availableNames = quizObject.AddComponent<Menu>();
            availableNames.rootMenu = new List<CustomMenuItem>();
            availableNames.alignment = Direction.VERTICAL;
            availableNames.markOnlyOne = true;
            availableNames.defaultMenuStyle = (GameObject) Resources.Load("QuizItem");
            for (int i = 0; i < annotationContainers.Count; i++)
            {
                CustomMenuItem item = quizObject.AddComponent<CustomMenuItem>();
                item.Init(null, null, false);
                string annotationText = annotationContainers[i].Annotation.Text;
                item.Text = annotationText;
                item.menuItemName = annotationText;
                item.subMenu = new List<CustomMenuItem>();
                item.onClickEvent.AddListener(delegate { OnItemClicked(annotationText); });
                item.markOnClick = true;
                availableNames.rootMenu.Add(item);
            }
        }
    }

    private void OnItemClicked(string text)
    {
        currentQuestion = text;
    }

    public void EvaluateQuestion(Annotation annotation)
    {
        EvaluateQuestion(annotation, currentQuestion);
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

    public override void OnDestroy()
    {
        base.OnDestroy();
        CleanUp();
    }

    private void CleanUp()
    {
        if (quizObject != null)
        {
            Destroy(quizObject);
        }
    }
}
