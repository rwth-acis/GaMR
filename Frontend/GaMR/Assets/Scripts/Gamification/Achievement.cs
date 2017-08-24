using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Contains all necessary information for an achievement object
/// </summary>
public class Achievement
{
    private string id;
    private string name;
    private string description;
    private int pointValue;
    private string badgeId;
    private bool notificationCheck;
    private string notificationMessage;

    public string ID { get { return id; } }
    public string Name { get { return name; } }
    public string Description { get { return description; } }
    public int PointValue { get { return pointValue; } set { pointValue = value; } }
    public bool NotificationCheck { get { return notificationCheck; } }
    public string NotificationMessage { get { return notificationMessage; } }

    public string BadgeId { get { return badgeId; } }

    public Achievement(string id, string name, string description, int pointValue) : this(id, name, description, pointValue, null, false, "")
    {
    }

    public Achievement(string id, string name, string description, int pointValue, string badgeId) : this(id, name, description, pointValue, badgeId, false, "")
    {
    }

    private Achievement(string id, string name, string description, int pointValue, string badgeId, string notificationMessage)
        : this(id, name, description, pointValue, badgeId, true, notificationMessage)
    {
    }

    private Achievement(string id, string name, string description, int pointValue, string badgeId, bool notificationCheck, string notificationMessage)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.pointValue = pointValue;
        this.badgeId = badgeId;
        this.notificationCheck = notificationCheck;
        this.notificationMessage = notificationMessage;
    }

    /// <summary>
    /// Converts the achievement to a multipart/form-data representation for POST and PUT queries
    /// It is designed to be compatible with the Gamification Framework
    /// </summary>
    /// <returns>The multipart/form-data with the correct fields</returns>
    public List<IMultipartFormSection> ToMultipartFormData()
    {
        List<IMultipartFormSection> body = new List<IMultipartFormSection>();
        body.Add(new MultipartFormDataSection("achievementid", ID));
        if (Name != "")
        {
            body.Add(new MultipartFormDataSection("achievementname", Name));
        }
        if (Description != "")
        {
            body.Add(new MultipartFormDataSection("achievementdesc", Description));
        }
        body.Add(new MultipartFormDataSection("achievementpointvalue", PointValue.ToString()));
        if (BadgeId != null && BadgeId != "")
        {
            body.Add(new MultipartFormDataSection("achievementbadgeid", BadgeId));
        }
        if (NotificationCheck)
        {
            body.Add(new MultipartFormDataSection("achievementnotificationcheck", true.ToString()));
        }
        if (NotificationMessage != "")
        {
            body.Add(new MultipartFormDataSection("achievementnotificationmessage", NotificationMessage));
        }

        return body;
    }

    public static Achievement FromJson(string json)
    {
        JsonAchievement jsonAchievement = JsonUtility.FromJson<JsonAchievement>(json);
        Achievement achievement = new Achievement(jsonAchievement.id, jsonAchievement.name, jsonAchievement.description, jsonAchievement.pointValue, jsonAchievement.badgeId, jsonAchievement.useNotification, jsonAchievement.notificationMessage);
        return achievement;
    }
}

[Serializable]
public class JsonAchievement
{
    public string id;
    public string name;
    public string description;
    public int pointValue;
    public string notificationMessage;
    public bool useNotification;
    public string badgeId;
}