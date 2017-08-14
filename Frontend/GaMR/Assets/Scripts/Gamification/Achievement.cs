using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public string NotificationMessage { get { return NotificationMessage; } }

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

}
