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

    public string CurrentlySelectedName { get; set; }
    public Annotation CurrentlySelectedAnnotation { get; set; }
    public AnnotationContainer CurrentlySelectedAnnotationContainer
    {
        get { return currentlySelectedAnnotationContainer; }
        set
        {
            if (currentlySelectedAnnotationContainer != null)
            {
                currentlySelectedAnnotationContainer.Deselect();
            }
            currentlySelectedAnnotationContainer = value;
        }
    }
    private AnnotationContainer currentlySelectedAnnotationContainer;
    private ObjectInfo objInfo;
    private GamificationManager gamificationManager;
    private Menu availableNames; // only used in the mode "name to position"
    private ProgressBar progressBar;
    List<int> freeIndices;
    List<IViewEvents> listeners = new List<IViewEvents>();

    BadgeManager badgeManager;

    private int correctlyAnswered = 0;

    /// <summary>
    /// The name of the quiz
    /// </summary>
    public string QuizName { get; set; }

    public GamificationManager GamificiationManager { get { return gamificationManager; } }

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
        gamificationManager = transform.parent.parent.GetComponent<GamificationManager>();
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

    private void EnsureQuizGamification()
    {
        Achievement createAchievement = new Achievement("Achi" + QuizName, QuizName, "", annotations.Count, "defaultBadge");

        gamificationManager.gameId = objInfo.ModelName.ToLower();

        // create or load quiz with achievement

        GamificationFramework.Instance.GetOrCreateAchievement(gamificationManager.gameId, createAchievement,
            resAchievement =>
            {
                if (resAchievement != null)
                {
                    Debug.Log("Achievement loaded or created");
                    gamificationManager.AchievementOfQuest = resAchievement;

                    // also load the badge
                    GamificationFramework.Instance.GetBadgeWithId(gamificationManager.gameId, resAchievement.BadgeId,
                        (resBadge, resCode) =>
                        {
                            if (resCode == 200)
                            {
                                Debug.Log("Badge loaded");

                                // load the image of the badge

                                GamificationFramework.Instance.GetBadgeImage(gamificationManager.gameId, resBadge.ID,
                                    (texture, resImageCode) =>
                                    {
                                        Debug.Log("Badge image loaded");
                                        if (resImageCode == 200)
                                        {
                                            resBadge.Image = (Texture2D)texture;
                                            gamificationManager.Badge = resBadge;
                                        }
                                    }
                                    );
                            }
                        }
                        );

                    // get or create the quest
                    Quest quizQuest = new Quest(QuizName, QuizName, QuestStatus.REVEALED, gamificationManager.AchievementOfQuest.ID, false, false, 0, "");

                    EnsureActionsForAnnotations(quizQuest);

                    quizQuest.AddAction("defaultAction", 1);

                    GamificationFramework.Instance.GetOrCreateQuest(gamificationManager.gameId, quizQuest,
                        resQuest =>
                        {
                            if (resQuest != null)
                            {
                                Debug.Log("Quest loaded");
                                gamificationManager.Quest = resQuest;
                            }
                            else
                            {
                                MessageBox.Show("Could not load gamification of the quiz", MessageBoxType.ERROR);
                            }
                        }
                        );
                }
                else
                {
                    MessageBox.Show("Could not load or create an achievement for the quiz", MessageBoxType.ERROR);
                }
            }
            );
    }

    public void ShowNames()
    {
        transform.parent.parent.GetComponentInChildren<QuizMenuSpawner>().enabled = true;
        ShowListeners();
    }

    private void EnsureActionsForAnnotations(Quest quest)
    {
        for (int i = 0; i < annotations.Count; i++)
        {
            int indexForLambda = i;
            GameAction action = GameAction.FromAnnotation(annotations[i], QuizName);
            quest.AddAction(action, 1);
            GamificationFramework.Instance.CreateAction(gamificationManager.gameId, action,
                resCode =>
                {
                    if (resCode != 201)
                    {
                        Debug.Log("Could not create action for " + annotations[indexForLambda].Text + "\nCode: " + resCode);
                    }
                }
                );
        }

        // also adapt achievement target points
        UpdateAchievementPoints();
    }

    /// <summary>
    /// Saves the quiz if the PlayerType is not STUDENT
    /// </summary>
    protected override void Save(bool synchronize)
    {
        if (InformationManager.Instance.playerType != PlayerType.STUDENT)
        {
            // if not student => save the quiz itself
            JsonAnnotationArray array = new JsonAnnotationArray();
            array.array = annotations;

            string jsonPost = JsonUtility.ToJson(array);
            RestManager.Instance.POST(InformationManager.Instance.FullBackendAddress + subPathSave + QuizName, jsonPost);

            // also save the gamification
            SaveGamification();
        }
    }

    private void SaveGamification()
    {
        gamificationManager.CommitQuest();
    }

    /// <summary>
    /// loads the annotations
    /// </summary>
    protected override void LoadAnnotations()
    {
        RestManager.Instance.GET(InformationManager.Instance.FullBackendAddress + subPathLoad + QuizName, QuizLoaded, null);
    }

    /// <summary>
    /// called when the quiz has finished loading
    /// </summary>
    /// <param name="res">The json string which is the result of the web request</param>
    private void QuizLoaded(UnityWebRequest res, object[] args)
    {
        Load(res, null);
        EnsureQuizGamification();
        if (InformationManager.Instance.playerType == PlayerType.STUDENT)
        {
            InitializeQuiz();
        }
        else
        {
            InitializeBadgeCreation();
        }
    }

    private void InitializeBadgeCreation()
    {
        Transform badgeHook = gameObject.transform.parent.parent.Find("FacePlayer");
        Quaternion currentRotation = badgeHook.localRotation;
        badgeHook.localRotation = Quaternion.identity;

        GameObject badgeObject = (GameObject)Instantiate(Resources.Load("Badge"));
        badgeObject.transform.parent = badgeHook;
        badgeObject.transform.position = gameObject.transform.position + new Vector3(-objInfo.Size.x, -objInfo.Size.y / 2f, 0);
        badgeHook.localRotation = currentRotation;

        badgeManager = badgeObject.GetComponent<BadgeManager>();

        gamificationManager.BadgeManager = badgeManager;
    }

    /// <summary>
    /// initializes the quiz
    /// randomly sets the mode and creates the necessary parts
    /// </summary>
    private void InitializeQuiz()
    {
        if (PositionToName == true)
        {
            MessageBox.Show(LocalizationManager.Instance.ResolveString("Click on the annotations and enter the name of the corresponding part"), MessageBoxType.INFORMATION);
        }
        else
        {
            MessageBox.Show(LocalizationManager.Instance.ResolveString("Connect the names with their corresponding position"), MessageBoxType.INFORMATION);

            transform.parent.parent.GetComponentInChildren<QuizMenuSpawner>().enabled = true;
        }

        // in both cases: create progress bar
        GameObject progressBarObject = (GameObject)Instantiate(Resources.Load("ProgressBar"));

        Transform boundingBoxHook = gameObject.transform.parent.parent.Find("MenuCenter");

        CirclePositioner positioner = progressBarObject.GetComponent<CirclePositioner>();
        positioner.boundingBox = boundingBoxHook;

        progressBar = progressBarObject.GetComponent<ProgressBar>();

        badgeManager = progressBar.Badge.GetComponent<BadgeManager>();
        gamificationManager.BadgeManager = badgeManager;
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
        GameAction action = GameAction.FromAnnotation(annotationContainer.Annotation, QuizName);
        GamificationFramework.Instance.CreateAction(gamificationManager.gameId, action,
            resCode =>
            {
                if (resCode != 200 && resCode != 201)
                {
                    Debug.Log("Could not gamify question (Code " + resCode + ")");
                    gamificationManager.Quest.AddAction(action, 1);
                }
            }
            );

        // also increase target for achievement
        UpdateAchievementPoints();
    }

    private void UpdateAchievementPoints()
    {
        gamificationManager.AchievementOfQuest.PointValue = annotations.Count;
        GamificationFramework.Instance.UpdateAchievement(gamificationManager.gameId, gamificationManager.AchievementOfQuest, null);
    }

    public override void Delete(AnnotationContainer annotationContainer)
    {
        base.Delete(annotationContainer);

        // handle gamification: delete action which is related to the question
        GamificationFramework.Instance.DeleteAction(gamificationManager.gameId, GameAction.CreateActionId(annotationContainer.Annotation, QuizName),
            resCode =>
            {
                if (resCode != 200)
                {
                    Debug.Log("Could not delete gamified question (Code " + resCode + ")");
                    gamificationManager.Quest.RemoveAction(annotationContainer.Annotation.Position.ToString() + "_" + QuizName);
                }
            }
            );

        // also decrease target for achievement
        UpdateAchievementPoints();
    }

    public override void NotifyAnnotationEdited(Annotation updatedAnnotation)
    {
        base.NotifyAnnotationEdited(updatedAnnotation);
        GameAction updatedAction = GameAction.FromAnnotation(updatedAnnotation, QuizName);
        GamificationFramework.Instance.UpdateAction(gamificationManager.gameId, updatedAction, null);
    }

    /// <summary>
    /// Evaluates a question by comparing the selected annotation with solution annotation currentContainer
    /// </summary>
    /// <param name="annotation">The selected annotation to compare</param>
    /// <returns></returns>
    public bool EvaluateQuestion(Annotation annotation)
    {
        if (CurrentlySelectedName != null && CurrentlySelectedName != "")
        {
            bool res = EvaluateQuestion(annotation, CurrentlySelectedName);
            return res;
        }
        else
        {
            return false;
        }
    }

    public bool EvaluateQuestion(string input)
    {
        if (CurrentlySelectedAnnotation != null)
        {
            bool res = EvaluateQuestion(CurrentlySelectedAnnotation, input);
            if (res)
            {
                CloseListeners();
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
    private bool EvaluateQuestion(Annotation annotation, string input)
    {
        if (annotation.Text == input)
        {
            MessageBox.Show(LocalizationManager.Instance.ResolveString("Correct"), MessageBoxType.SUCCESS);
            correctlyAnswered++;
            progressBar.Progress = (float)correctlyAnswered / annotations.Count;

            annotation.Answered = true;
            InformListeners();
            CurrentlySelectedName = "";


            GamificationFramework.Instance.TriggerAction(gamificationManager.gameId, GameAction.CreateActionId(annotation, QuizName));

            if (progressBar.Progress == 1)
            {
                CloseListeners();

                // also trigger the default action
                GamificationFramework.Instance.TriggerAction(gamificationManager.gameId, "defaultAction");
                badgeManager.WinBadge();
            }

            return true;
        }
        else
        {
            MessageBox.Show(LocalizationManager.Instance.ResolveString("Incorrect"), MessageBoxType.ERROR);
            return false;
        }
    }

    public void AddListener(IViewEvents listener)
    {
        listeners.Add(listener);
    }

    public void RemoveListener(IViewEvents listener)
    {
        listeners.Remove(listener);
    }

    private void InformListeners()
    {
        foreach (IViewEvents listener in listeners)
        {
            listener.UpdateView();
        }
    }

    private void ShowListeners()
    {
        foreach (IViewEvents listener in listeners)
        {
            listener.Show();
        }
    }

    private void CloseListeners()
    {
        foreach (IViewEvents listener in listeners)
        {
            listener.Close();
        }
    }

    private void DestroyListeners()
    {
        foreach (IViewEvents listener in listeners)
        {
            listener.Destroy();
        }
    }

    /// <summary>
    /// called when the QuizManager is destroyed
    /// calls the base method to save the annotations
    /// cleans up remaining quiz components
    /// </summary>
    public override void OnDestroy()
    {
        transform.parent.parent.GetComponentInChildren<QuizMenuSpawner>().enabled = false;
        DestroyListeners();
        base.OnDestroy();
        CleanUp();
    }

    /// <summary>
    /// cleans up remaining items of the quiz
    /// </summary>
    private void CleanUp()
    {
        CloseListeners();
        if (progressBar != null)
        {
            Destroy(progressBar.gameObject);
        }
        if (badgeManager != null)
        {
            Destroy(badgeManager.gameObject);
        }
    }
}
