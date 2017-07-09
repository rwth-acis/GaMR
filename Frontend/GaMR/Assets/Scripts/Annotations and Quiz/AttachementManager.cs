using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// coordinates Annotation- and QuizManagers so that an object can only have one
/// </summary>
public class AttachementManager : MonoBehaviour
{
    /// <summary>
    /// the current manager
    /// </summary>
    private AnnotationManager manager;
    /// <summary>
    /// notifiers which can register user input
    /// </summary>
    private TapNotifier[] notifiers;

    /// <summary>
    /// initialization and set an AnnotationManager
    /// </summary>
    public void Init()
    {
        notifiers = GetComponentsInChildren<TapNotifier>();
        SetAnnotationManager();
    }

    /// <summary>
    /// whether a QuizManager is currently attached to the object
    /// </summary>
    public bool IsQuiz { get; private set; }

    /// <summary>
    /// Sets an AnnotationManager as the current manager
    /// </summary>
    /// <returns>the attached AnnotationManager instance</returns>
    public AnnotationManager SetAnnotationManager()
    {
        return SetManager(false, "");
    }

    /// <summary>
    /// Sets a QuizManager as the current manager
    /// </summary>
    /// <param name="quizName">The name of the quiz to load</param>
    /// <returns>the attached QuizManager instance</returns>
    public AnnotationManager SetQuizManager(string quizName)
    {
        return SetManager(true, quizName);
    }

    /// <summary>
    /// general method for changing the manager instance
    /// replaces the old instace with the new one
    /// </summary>
    /// <param name="isQuiz">if true a QuizManager is created instead of a AnnotationManager</param>
    /// <param name="quizName">if isQuiz this is used to tell the QuizManager which quiz to load</param>
    /// <returns></returns>
    private AnnotationManager SetManager(bool isQuiz, string quizName)
    {
        if (manager != null)
        {
            UnRegisterOnNotifiers();
            manager.HideAllAnnotations();
            Destroy(manager);
        }
        if (isQuiz)
        {
            QuizManager quizManager = gameObject.AddComponent<QuizManager>();
            quizManager.QuizName = quizName;
            manager = quizManager;
            IsQuiz = true;
        }
        else
        {
            manager = gameObject.AddComponent<AnnotationManager>();
            IsQuiz = false;
        }
        RegisterOnNotifiers();
        return manager;
    }

    /// <summary>
    /// removes tap-function of the current manager from the list of methods to notify about user input
    /// </summary>
    private void UnRegisterOnNotifiers()
    {
        foreach(TapNotifier notifier in notifiers)
        {
            notifier.UnRegisterListenerOnInputDown(manager.TapOnModel);
        }
    }

    /// <summary>
    /// adds tap-function of the current manager from the list of methods to notify about user input
    /// </summary>
    private void RegisterOnNotifiers()
    {
        foreach (TapNotifier notifier in notifiers)
        {
            notifier.RegisterListenerOnInputDown(manager.TapOnModel);
        }
    }

    /// <summary>
    /// the current Annotation- or QuizManager
    /// </summary>
    public AnnotationManager Manager { get { return manager; } }

    /// <summary>
    /// whether or not the current manager is in editMode
    /// </summary>
    public bool EditMode
    {
        get { return manager.EditMode; }
        set { manager.EditMode = value; }

    }
}
