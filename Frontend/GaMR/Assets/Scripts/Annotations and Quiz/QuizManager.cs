using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Administers the quiz and the quiz display
/// </summary>
public class QuizManager : AnnotationManager
{
    protected new string subPathLoad = "/resources/quiz/load/";
    protected new string subPathSave = "/resources/quiz/save/";

    private AnnotationContainer currentContainer;
    private CustomMenuItem currentMenuItem;
    private ObjectInfo objInfo;
    private GameObject quizObject;
    private Menu availableNames; // only used in the mode "name to position"

    /// <summary>
    /// The name of the quiz
    /// </summary>
    public string QuizName { get; set; }

    /// <summary>
    /// if true: quiz is in the mode where the position is given and the user has to enter the name
    /// if false: quiz is in the mode where the names are given and the user has to find the position
    /// </summary>
    public bool PositionToName { get; set; }

    /// <summary>
    /// initializes the quiz and loads the quiz elements
    /// </summary>
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

    /// <summary>
    /// Saves the quiz if the PlayerType is not STUDENT
    /// </summary>
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

    /// <summary>
    /// loads the annotations
    /// </summary>
    protected override void LoadAnnotations()
    {
        restManager.GET(infoManager.BackendAddress + subPathLoad + QuizName, QuizLoaded);
    }

    /// <summary>
    /// called when the quiz has finished loading
    /// </summary>
    /// <param name="res">The json string which is the result of the web request</param>
    private void QuizLoaded(string res)
    {
        Load(res);
        if (InformationManager.instance.playerType == PlayerType.STUDENT)
        {
            InitializeQuiz();
        }
    }

    /// <summary>
    /// initializes the quiz
    /// randomly sets the mode and creates the necessary parts
    /// </summary>
    private void InitializeQuiz()
    {
        int value = UnityEngine.Random.Range(0, 2);
        if (value == 0)
        {
            PositionToName = true;
            MessageBox.Show(LocalizationManager.Instance.ResolveString("Click on the annotations and enter the name of the corresponding part"), MessageBoxType.INFORMATION);
        }
        else
        {
            PositionToName = false;
            MessageBox.Show(LocalizationManager.Instance.ResolveString("Connect the names with their corresponding position"), MessageBoxType.INFORMATION);
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

    /// <summary>
    /// Called when an item is clicked
    /// </summary>
    /// <param name="name">The name of the item</param>
    private void OnItemClicked(string name)
    {
        // text should be the index since the item was initialized this way
        int index = int.Parse(name);
        currentContainer = annotationContainers[index];
        // availableNames is not null or else this could not be called
        currentMenuItem = availableNames.rootMenu[index];
    }

    /// <summary>
    /// Evaluates a question by comparing the selected annotation with solution annotation currentContainer
    /// </summary>
    /// <param name="annotation">The selected annotation to compare</param>
    /// <returns></returns>
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

    /// <summary>
    /// evaluates a question by comparing the annotation with the input text
    /// Shows a MessageBox to indicate success or failure
    /// </summary>
    /// <param name="annotation">The selected annotation</param>
    /// <param name="input">The user input to compare to the annotation's text</param>
    /// <returns>true if input is equal to the annotations text, else false</returns>
    public bool EvaluateQuestion(Annotation annotation, string input)
    {
        if (annotation.Text == input)
        {
            MessageBox.Show(LocalizationManager.Instance.ResolveString("Correct"), MessageBoxType.SUCCESS);
            return true;
        }
        else
        {
            MessageBox.Show(LocalizationManager.Instance.ResolveString("Incorrect"), MessageBoxType.ERROR);
            return false;
        }
    }

    /// <summary>
    /// called when the QuizManager is destroyed
    /// calls the base method to save the annotations
    /// cleans up remaining quiz components
    /// </summary>
    public override void OnDestroy()
    {
        base.OnDestroy();
        CleanUp();
    }

    /// <summary>
    /// cleans up remaining items of the quiz
    /// </summary>
    private void CleanUp()
    {
        if (quizObject != null)
        {
            Destroy(quizObject);
        }
    }
}
