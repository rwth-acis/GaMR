using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizManager : AnnotationManager
{
    protected new string subPathLoad = "/resources/quiz/load/";
    protected new string subPathSave = "/resources/quiz/save/";

    private AnnotationContainer currentContainer;
    private CustomMenuItem currentMenuItem;
    private ObjectInfo objInfo;
    private GameObject quizObject;
    private Menu availableNames; // only used in the mode "name to position"

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

            availableNames = quizObject.AddComponent<Menu>();
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
                string index = i.ToString();
                item.menuItemName = i.ToString();
                item.subMenu = new List<CustomMenuItem>();
                item.onClickEvent.AddListener(OnItemClicked);
                item.markOnClick = true;
                availableNames.rootMenu.Add(item);
            }
        }
    }

    private void OnItemClicked(string text)
    {
        // text should be the index since the item was initialized this way
        int index = int.Parse(text);
        currentContainer = annotationContainers[index];
        // availableNames is not null or else this could not be called
        currentMenuItem = availableNames.rootMenu[index];
    }

    public bool EvaluateQuestion(Annotation annotation)
    {
        if (currentContainer != null)
        {
            bool res = EvaluateQuestion(annotation, currentContainer.Annotation.Text);
            if (res)
            {
                currentMenuItem.Destroy();
            }
            return res;
        }
        else
        {
            return false;
        }
    }

    public bool EvaluateQuestion(Annotation annotation, string input)
    {
        if (annotation.Text == input)
        {
            MessageBox.Show("Correct", MessageBoxType.SUCCESS);
            return true;
        }
        else
        {
            MessageBox.Show("Incorrect", MessageBoxType.ERROR);
            return false;
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
