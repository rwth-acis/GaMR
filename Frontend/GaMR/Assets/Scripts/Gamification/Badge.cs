using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Badge
{
    private string id;
    private string name;
    private string description;
    private bool notificationCheck;
    private string notificationMessage;

    public string ID { get { return id; } }
    public string Name { get { return name; } }

    public string Description { get { return description; } }

    public bool NotificationCheck { get { return notificationCheck; } }

    public string NotificationMessage { get { return NotificationMessage; } }

    public Badge(string id, string name, string description, string notificationMessage) : this(id, name, description, true, notificationMessage)
    {

    }

    public Badge(string id, string name, string description) : this(id, name, description, false, "")
    {

    }

    private Badge(string id, string name, string description, bool notificationCheck, string notificationMessage)
    {

    }
}
