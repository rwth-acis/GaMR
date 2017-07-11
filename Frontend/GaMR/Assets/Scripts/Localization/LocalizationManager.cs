using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    private static LocalizationManager instance;

    private Dictionary<string, string> dictionary;
    private Language language;

    public void Start()
    {
        language = InformationManager.instance.Language;
        LoadDictionary();

        if (instance == null)
        {
            instance = this;
        }
    }

    private void LoadDictionary()
    {
        dictionary = new Dictionary<string, string>();
        TextAsset jsonData = Resources.Load<TextAsset>("values/strings-" + GetLanguageCode(language));
        JsonStringItemArray array = JsonUtility.FromJson<JsonStringItemArray>(jsonData.text);
        foreach(StringItem item in array.strings)
        {
            if (!dictionary.ContainsKey(item.key))
            {
                dictionary.Add(item.key, item.value);
            }
            else
            {
                Debug.LogError("The language file for " + GetLanguageCode(language) + " contains the same name-id multiple times: " + item.key);
            }
        }
    }

    public static LocalizationManager Instance
    {
        get { return instance; }
    }

    public string ResolveString(string text)
    {
        if (dictionary.ContainsKey(text))
        {
            return dictionary[text];
        }
        else
        {
            Debug.LogWarning("Requested string could not be translated: " + text);
            return text;
        }
    }

    private string GetLanguageCode(Language language)
    {
        switch (language)
        {
            case Language.ENGLISH:
                return "en";
            case Language.GERMAN:
                return "de";
            default:
                return "en";
        }
    }
}
