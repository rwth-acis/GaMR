using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Represents an action in the Gamification Framework
/// </summary>
public class GameAction
{
    private string id;
    private string name;
    private string description;
    private int pointValue;
    private bool notificationCheck;
    private string notificationMessage;

    public string ID { get { return id; } }

    public string Name { get { return name; } }

    public string Description { get { return description; } }

    public int PointValue { get { return pointValue; } }

    public bool NotificationCheck { get { return notificationCheck; } }

    public string NotificationMessage { get { return notificationMessage; } }

    public GameAction(string id, string name, string description, int pointValue) : this(id, name, description, pointValue, false, "")
    {

    }

    public GameAction(string id, string name, string description, int pointValue, string notificationMessage) : this(id, name, description, pointValue, true, notificationMessage)
    {

    }

    private GameAction(string id, string name, string description, int pointValue, bool notificationCheck, string notificationMessage)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.pointValue = pointValue;
        this.notificationCheck = notificationCheck;
        this.notificationMessage = notificationMessage;
    }

    /// <summary>
    /// Converts the action to multipart/form-data in order to perform POST and PUT queries with the data
    /// It is designed to be compatible with the Gamfication Framework.
    /// </summary>
    /// <returns>The multipart/form-data with the filled fields</returns>
    public List<IMultipartFormSection> ToMultipartFormData()
    {
        List<IMultipartFormSection> body = new List<IMultipartFormSection>();
        body.Add(new MultipartFormDataSection("actionid", ID));
        if (Name != "")
        {
            body.Add(new MultipartFormDataSection("actionname", Name));
        }
        if (Description != "")
        {
            body.Add(new MultipartFormDataSection("actiondesc", Description));
        }
        body.Add(new MultipartFormDataSection("actionpointvalue", PointValue.ToString()));
        if (NotificationCheck)
        {
            body.Add(new MultipartFormDataSection("actionnotificationcheck", true.ToString()));
        }
        if (NotificationMessage != "")
        {
            body.Add(new MultipartFormDataSection("actionnotificationmessage", NotificationMessage));
        }

        return body;
    }

}
