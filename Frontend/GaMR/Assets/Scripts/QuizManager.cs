using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizManager : AnnotationManager
{
    public string QuizName { get; set; }

    protected new void Save()
    {
        JsonArray<Annotation> array = new JsonArray<Annotation>();
        array.array = annotations;

        string jsonPost = JsonUtility.ToJson(array);
        if (restManager != null)
        {
            restManager.POST(infoManager.BackendAddress + "/resources/quiz/save/" + objectInfo.ModelName + "/" + QuizName, jsonPost);
        }
    }

    protected new void LoadAnnotations()
    {
        restManager.GET(infoManager.BackendAddress + "/resources/quiz/load/" + objectInfo.ModelName + "/" + QuizName, Load);
    }


}
