﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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

    public string NotificationMessage { get { return notificationMessage; } }

    public Texture2D Image { get { return image; } set { image = value; } }

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

    public List<IMultipartFormSection> ToMultipartForm()
    {
        List<IMultipartFormSection> body = new List<IMultipartFormSection>();
        body.Add(new MultipartFormDataSection("badgeid", ID));
        if (Name != "")
        {
            body.Add(new MultipartFormDataSection("badgename", Name));
        }
        if (Description != "")
        {
            body.Add(new MultipartFormDataSection("badgedesc", Description));
        }
        if (NotificationCheck)
        {
            body.Add(new MultipartFormDataSection("badgenotificationcheck", "true"));
        }
        if (NotificationMessage != "")
        {
            body.Add(new MultipartFormDataSection("badgenotificationmessage", NotificationMessage));
        }
        if (Image != null)
        {
            body.Add(new MultipartFormFileSection("badgeimageinput", Image.GetRawTextureData(), Name + ".jpg", "file"));
        }
        body.Add(new MultipartFormDataSection("dev", "true")); // this is needed so that the Gamification Framework accepts the image

        return body;
    }
}
