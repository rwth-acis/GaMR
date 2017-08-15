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
    private Texture2D image;

    public string ID { get { return id; } }
    public string Name { get { return name; } }

    public string Description { get { return description; } }

    public bool NotificationCheck { get { return notificationCheck; } }

    public string NotificationMessage { get { return NotificationMessage; } }

    public Texture2D Image { get { return image; } }

    public Badge(string id, string name, string description, string notificationMessage) : this(id, name, description, true, notificationMessage)
    {

    }

    public Badge(string id, string name, string description) : this(id, name, description, false, "")
    {

    }

    private Badge(string id, string name, string description, bool notificationCheck, string notificationMessage)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.notificationCheck = notificationCheck;
        this.notificationMessage = notificationMessage;
    }

    public WWWForm ToWWWForm()
    {
        WWWForm body = new WWWForm();
        body.AddField("badgeid", ID);
        body.AddField("badgename", Name);
        body.AddField("badgedesc", Description);
        if (NotificationCheck)
        {
            // if the field exists, the bool variable will be set to true in the framework (no matter which value the www-field has)
            body.AddField("badgenotificationcheck", "true");
        }
        body.AddField("badgenotificationmessage", NotificationMessage);
        body.AddBinaryData("badgeimageinput", Image.GetRawTextureData());

        return body;
    }
}
