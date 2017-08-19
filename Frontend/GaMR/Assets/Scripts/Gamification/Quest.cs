using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    private string id;
    private string name;
    private QuestStatus status;
    private Achievement achievement;
    private bool questflag;
    private bool pointflag;
    private int pointValue;
    private Dictionary<GameAction, int> actions;
    private string description;
    private bool notificationcheck;
    private string notificationmessage;

    public Quest(string id, string name, QuestStatus status, Achievement achievement, bool questflag, bool pointflag,
        int pointValue, Dictionary<GameAction, int> actions, string description) : this(id, name, status, achievement, questflag, pointflag, pointValue, actions, description, false, "")
    {
        
    }

    public Quest(string id, string name, QuestStatus status, Achievement achievement, bool questflag, bool pointflag,
        int pointValue, string description) : this(id, name, status, achievement, questflag, pointflag, pointValue, new Dictionary<GameAction, int>(), description, false, "")
    {

    }

    public Quest(string id, string name, QuestStatus status, Achievement achievement, bool questflag,
        bool pointflag, int pointValue, string description, bool notificationcheck, string notificationmessage)
        : this(id, name, status, achievement, questflag, pointflag, pointValue, new Dictionary<GameAction, int>(), description, notificationcheck, notificationmessage)
    {

    }

    public Quest(string id, string name, QuestStatus status, Achievement achievement, bool questflag,
        bool pointflag, int pointValue, Dictionary<GameAction, int> actions, string description, bool notificationcheck, string notificationmessage)
    {
        this.id = id;
        this.name = name;
        this.status = status;
        this.achievement = achievement;
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
        actions.Add(action, maxNumberOfTriggers);
    }

    public string ToJson()
    {
        JsonQuest jsonObject = new JsonQuest();
        jsonObject.questid = id;
        jsonObject.questname = name;
        jsonObject.queststatus = status.ToString();
        jsonObject.questachievementid = achievement.ID;
        jsonObject.questquestflag = questflag;
        jsonObject.questpointflag = pointflag;
        jsonObject.questpointvalue = pointValue;
        List<JsonAction> actionIds = new List<JsonAction>();
        foreach(KeyValuePair<GameAction, int> action in actions)
        {
            actionIds.Add(new JsonAction(action.Key.ID, action.Value));
        }
        jsonObject.questactionids = actionIds;
        jsonObject.questdescription = description;
        jsonObject.questnotificationcheck = notificationcheck;
        jsonObject.questnotificationmessage = notificationmessage;

        string json = JsonUtility.ToJson(jsonObject);

        return json;

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