using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    private string id;
    private string name;
    private QuestStatus status;
    private string achievementId;
    private bool questflag;
    private bool pointflag;
    private int pointValue;
    private Dictionary<string, int> actions;
    private string description;
    private bool notificationcheck;
    private string notificationmessage;

    public string ID
    {
        get { return id; }
    }

    public Quest(string id, string name, QuestStatus status, string achievementId, bool questflag, bool pointflag,
        int pointValue, Dictionary<string, int> actions, string description) : this(id, name, status, achievementId, questflag, pointflag, pointValue, actions, description, false, "")
    {
        
    }

    public Quest(string id, string name, QuestStatus status, string achievementId, bool questflag, bool pointflag,
        int pointValue, string description) : this(id, name, status, achievementId, questflag, pointflag, pointValue, new Dictionary<string, int>(), description, false, "")
    {

    }

    public Quest(string id, string name, QuestStatus status, string achievementId, bool questflag,
        bool pointflag, int pointValue, string description, bool notificationcheck, string notificationmessage)
        : this(id, name, status, achievementId, questflag, pointflag, pointValue, new Dictionary<string, int>(), description, notificationcheck, notificationmessage)
    {

    }

    public Quest(string id, string name, QuestStatus status, string achievementId, bool questflag,
        bool pointflag, int pointValue, Dictionary<string, int> actions, string description, bool notificationcheck, string notificationmessage)
    {
        this.id = id;
        this.name = name;
        this.status = status;
        this.achievementId = achievementId;
        this.questflag = questflag;
        this.pointflag = pointflag;
        this.pointValue = pointValue;
        this.actions = actions;
        this.description = description;
        this.notificationcheck = notificationcheck;
        this.notificationmessage = notificationmessage;
    }

    public void AddAction(GameAction action, int maxNumberOfTriggers)
    {
        AddAction(action.ID, maxNumberOfTriggers);
    }

    public void AddAction(string actionId, int maxNumberOfTriggers)
    {
        actions.Add(actionId, maxNumberOfTriggers);
    }

    public void RemoveAction(string actionId)
    {
        actions.Remove(actionId);
    }

    public string ToJson()
    {
        JsonQuest jsonObject = new JsonQuest();
        jsonObject.questid = id;
        jsonObject.questname = name;
        jsonObject.queststatus = status.ToString();
        jsonObject.questachievementid = achievementId;
        jsonObject.questquestflag = questflag;
        jsonObject.questpointflag = pointflag;
        jsonObject.questpointvalue = pointValue;
        List<JsonAction> actionIds = new List<JsonAction>();
        foreach(KeyValuePair<string, int> action in actions)
        {
            actionIds.Add(new JsonAction(action.Key, action.Value));
        }
        jsonObject.questactionids = actionIds;
        jsonObject.questdescription = description;
        jsonObject.questnotificationcheck = notificationcheck;
        jsonObject.questnotificationmessage = notificationmessage;

        string json = JsonUtility.ToJson(jsonObject);

        Debug.Log(json);

        return json;

    }

    public static Quest FromJson(string json)
    {
        JsonResponseQuest jsonQuest = JsonUtility.FromJson<JsonResponseQuest>(json);
        QuestStatus status = (QuestStatus)Enum.Parse(typeof(QuestStatus),jsonQuest.status);
        Quest quest = new Quest(jsonQuest.id, jsonQuest.name, status, jsonQuest.achievementId, jsonQuest.questFlag, jsonQuest.pointFlag, jsonQuest.pointValue, jsonQuest.description, jsonQuest.useNotification, jsonQuest.notificationMessage);
        foreach(JsonResponseAction action in jsonQuest.actionIds)
        {
            quest.AddAction(action.key, action.value);
        }
        return quest;
    }
}

public enum QuestStatus
{
    REVEALED
}

[Serializable]
public class JsonQuest
{
    public string questid;
    public string questname;
    public string queststatus;
    public string questachievementid;
    public bool questquestflag;
    public bool questpointflag;
    public int questpointvalue;
    public List<JsonAction> questactionids;
    public string questdescription;
    public bool questnotificationcheck;
    public string questnotificationmessage;
}

[Serializable]
public class JsonResponseQuest
{
    public string name;
    public string description;
    public string status;
    public string id;
    public List<JsonResponseAction> actionIds;
    public string achievementId;
    public bool questFlag;
    public bool pointFlag;
    public string questIdCompleted;
    public int pointValue;
    public string notificationMessage;
    public bool useNotification;
}

[Serializable]
public class JsonAction
{
    public string action;
    public int times;

    public JsonAction(string action, int times)
    {
        this.action = action;
        this.times = times;
    }
}

[Serializable]
public class JsonResponseAction
{
    public string left;
    public int right;
    public int value;
    public string key;
}