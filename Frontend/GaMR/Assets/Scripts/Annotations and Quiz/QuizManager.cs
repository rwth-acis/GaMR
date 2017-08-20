using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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
    private ProgressBar progressBar;
    List<int> freeIndices;

    private int correctlyAnswered = 0;

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

        if (InformationManager.Instance.playerType == PlayerType.STUDENT)
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
        if (InformationManager.Instance.playerType != PlayerType.STUDENT)
        {
            // if not student => save the quiz itself
            JsonAnnotationArray array = new JsonAnnotationArray();
            array.array = annotations;

            string jsonPost = JsonUtility.ToJson(array);
            if (restManager != null)
            {
                restManager.POST(infoManager.BackendAddress + subPathSave + QuizName, jsonPost);
            }

            // also save the gamification
            SaveGamification();
        }
        else // if it is a student => save the achievements
        {

        }
    }

    private void SaveGamification()
    {
    }

    /// <summary>
    /// loads the annotations
    /// </summary>
    protected override void LoadAnnotations()
    {
        restManager.GET(infoManager.BackendAddress + subPathLoad + QuizName, QuizLoaded, null);
    }

    /// <summary>
    /// called when the quiz has finished loading
    /// </summary>
    /// <param name="res">The json string which is the result of the web request</param>
    private void QuizLoaded(UnityWebRequest res, object[] args)
    {
        Load(res, null);
        if (InformationManager.Instance.playerType == PlayerType.STUDENT)
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

        // get the boundingBoxHook: it always faces the player
        // this will be used to correctly align the quiz options and the progress bar
        Transform boundingBoxHook = gameObject.transform.parent.parent.Find("FacePlayer");
        // reset the rotation in order to position the progress bar at the right point
        Quaternion currentRotation = boundingBoxHook.localRotation;
        boundingBoxHook.localRotation = Quaternion.identity;


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
            quizObject.AddComponent<RotateToCameraOnYAxis>();
            quizObject.transform.parent = boundingBoxHook;
            quizObject.transform.position = gameObject.transform.position + new Vector3(objInfo.Size.x,objInfo.Size.y/2, 0);

            availableNames = quizObject.AddComponent<Menu>();
            availableNames.rootMenu = new List<CustomMenuItem>();
            availableNames.alignment = Direction.VERTICAL;
            availableNames.markOnlyOne = true;
            availableNames.defaultMenuStyle = (GameObject) Resources.Load("QuizItem");

            InitializeFreeIndices();

            // only take 5 questions at first
            for (int i = 0; i < 5; i++)
            {
                CustomMenuItem item = quizObject.AddComponent<CustomMenuItem>();
                item.Init(null, null, false);

                int randomIndex = UnityEngine.Random.Range(0, freeIndices.Count);
                int annotationIndex = freeIndices[randomIndex];


                string annotationText = annotations[annotationIndex].Text;
                item.Text = annotationText;

                freeIndices.RemoveAt(randomIndex);

                string index = annotationIndex.ToString();
                item.MenuItemName = index;
                item.subMenu = new List<CustomMenuItem>();
                item.onClickEvent.AddListener(OnItemClicked);
                item.markOnClick = true;
                availableNames.rootMenu.Add(item);
            }
        }

        // in both cases: create progress bar
        GameObject progressBarObject = (GameObject)Instantiate(Resources.Load("ProgressBar"));

        progressBarObject.transform.parent = boundingBoxHook;

        progressBarObject.transform.position = gameObject.transform.position + new Vector3(-objInfo.Size.x, - objInfo.Size.y/2f, 0);

        // set the rotation to the value it had before
        boundingBoxHook.localRotation = currentRotation;

        progressBar = progressBarObject.GetComponent<ProgressBar>();
    }

    private void InitializeFreeIndices()
    {
        freeIndices = new List<int>();
        for (int i = 0; i < annotations.Count; i++)
        {
            freeIndices.Add(i);
        }
    }

    public override void Add(AnnotationContainer annotationContainer)
    {
        base.Add(annotationContainer);

        // handle gamification: add question as action
        GameAction action = new GameAction(annotationContainer.Annotation.Position.ToString(), annotationContainer.Annotation.Text, "", 1);
        GamificationFramework.Instance.CreateAction(objInfo.ModelName, action,
            resCode =>
            {
                if (resCode != 200 || resCode != 201)
                {
                    Debug.Log("Could not gamify question (Code " + resCode + ")");
                }
            }
            );
    }

    public override void Delete(AnnotationContainer annotationContainer)
    {
        base.Delete(annotationContainer);

        // handle gamification: delete action which is related to the question
        GamificationFramework.Instance.DeleteAction(objInfo.ModelName, annotationContainer.Annotation.Position.ToString(),
            resCode =>
            {
                if (resCode != 200)
                {
                    Debug.Log("Could not delete gamified question (Code " + resCode + ")");
                }
            }
            );
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
        currentMenuItem = availableNames.GetItem(name);
    }

    /// <summary>
    /// Evaluates a question by comparing the selected annotation with solution annotation currentContainer
    /// </summary>
    /// <param name="annotation">The selected annotation to compare</param>
    /// <returns></returns>
    public bool EvaluateQuestion(Annotation annotation)
    {
        if (currentContainer != null && currentMenuItem != null)
        {
            bool res = EvaluateQuestion(annotation, currentContainer.Annotation.Text);
            if (res) // answer was correct
            {
                // if there are still annotations to ask => change the menu item
                if (freeIndices.Count > 0)
                {
                    int randomIndex = UnityEngine.Random.Range(0, freeIndices.Count);
                    currentMenuItem.Text = annotations[freeIndices[randomIndex]].Text;
                    currentMenuItem.MenuItemName = freeIndices[randomIndex].ToString();
                    currentMenuItem.Marked = false;
                    freeIndices.RemoveAt(randomIndex);
                }
                else
                {
                    currentMenuItem.Destroy();
                }
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
            correctlyAnswered++;
            progressBar.Progress = (float)correctlyAnswered / annotations.Count;
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
        if (progressBar != null)
        {
            Destroy(progressBar.gameObject);
        }
    }
}
