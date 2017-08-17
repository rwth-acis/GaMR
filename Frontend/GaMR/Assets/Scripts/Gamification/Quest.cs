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
    private List<GameAction> actions;
    private string description;
    private bool notificationcheck;
    private string notificationmessage;

    public Quest(string id, string name, QuestStatus status, Achievement achievement, bool questflag, bool pointflag,
        int pointValue, List<GameAction> actions, string description) : this(id, name, status, achievement, questflag, pointflag, pointValue, actions, description, false, "")
    {
        
    }

    public Quest(string id, string name, QuestStatus status, Achievement achievement, bool questflag, bool pointflag,
        int pointValue, string description) : this(id, name, status, achievement, questflag, pointflag, pointValue, new List<GameAction>(), description, false, "")
    {

    }

    public Quest(string id, string name, QuestStatus status, Achievement achievement, bool questflag,
        bool pointflag, int pointValue, string description, bool notificationcheck, string notificationmessage)
        : this(id, name, status, achievement, questflag, pointflag, pointValue, new List<GameAction>(), description, notificationcheck, notificationmessage)
    {

    }

    public Quest(string id, string name, QuestStatus status, Achievement achievement, bool questflag,
        bool pointflag, int pointValue, List<GameAction> actions, string description, bool notificationcheck, string notificationmessage)
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

    public void AddAction(GameAction action)
    {
        actions.Add(action);
    }

    public string ToJson()
    {
        throw new NotImplementedException();
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
    public string questStatus;
    public string questachievementid;
    public bool questquestflag;
    public bool questpointflag;
    public int questpointvalue;
    public List<string> questactionids;
    public string questdescription;
    public bool questnotificationcheck;
    public string questnotificationmessage;
}