using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachementManager : MonoBehaviour
{

    private AnnotationManager manager;
    private TapNotifier[] notifiers;

    // Use this for initialization
    public void Init()
    {
        notifiers = GetComponentsInChildren<TapNotifier>();
        SetAnnotationManager();
    }

    public bool IsQuiz { get; private set; }

    public AnnotationManager SetAnnotationManager()
    {
        return SetManager(false, "");
    }

    public AnnotationManager SetQuizManager(string quizName)
    {
        return SetManager(true, quizName);
    }

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


    private void UnRegisterOnNotifiers()
    {
        foreach(TapNotifier notifier in notifiers)
        {
            notifier.UnRegisterListenerOnInputDown(manager.TapOnModel);
        }
    }

    private void RegisterOnNotifiers()
    {
        foreach (TapNotifier notifier in notifiers)
        {
            notifier.RegisterListenerOnInputDown(manager.TapOnModel);
        }
    }

    public AnnotationManager Manager { get { return manager; } }

    public bool EditMode
    {
        get { return manager.EditMode; }
        set { manager.EditMode = value; }

    }
}
