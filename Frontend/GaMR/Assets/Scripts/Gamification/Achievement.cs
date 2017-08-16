using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Achievement
{
    private string id;
    private string name;
    private string description;
    private int pointValue;
    private Badge badge;
    private bool notificationCheck;
    private string notificationMessage;

    public string ID { get { return id; } }
    public string Name { get { return name; } }
    public string Description { get { return description; } }
    public int PointValue { get { return pointValue; } }
    public bool NotificationCheck { get { return notificationCheck; } }
    public string NotificationMessage { get { return notificationMessage; } }

    public Badge Badge { get { return badge; } }

    public Achievement(string id, string name, string description, int pointValue) : this(id, name, description, pointValue, null, false, "")
    {
    }

    public Achievement(string id, string name, string description, int pointValue, Badge badge) : this(id, name, description, pointValue, badge, false, "")
    {
    }

    private Achievement(string id, string name, string description, int pointValue, Badge badge, string notificationMessage)
        : this(id, name, description, pointValue, badge, true, notificationMessage)
    {
    }

    private Achievement(string id, string name, string description, int pointValue, Badge badge, bool notificationCheck, string notificationMessage)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.pointValue = pointValue;
        this.badge = badge;
        this.notificationCheck = notificationCheck;
        this.notificationMessage = notificationMessage;
    }

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
        if (Badge != null)
        {
            body.Add(new MultipartFormDataSection("achievementbadgeid", Badge.ID));
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

}
