using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public WWWForm ToWWWForm()
    {
        WWWForm body = new WWWForm();
        body.AddField("actionid", ID);
        body.AddField("actionname", Name);
        body.AddField("actiondesc", Description);
        body.AddField("actionpointvalue", PointValue);
        if (NotificationCheck)
        {
            // if the field exists, the bool variable will be set to true in the framework (no matter which value the www-field has)
            body.AddField("actionnotificationcheck", "true");
        }
        body.AddField("actionnotificationmessage", NotificationMessage);

        return body;
    }

}
