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

    public string ID { get { return id; } }
    public string Name { get { return name; } }
    public string Description { get { return description; } }
    public int PointValue { get { return pointValue; } }

    public Badge Badge { get { return badge; } }

    public Achievement(string id, string name, string description, int pointValue) : this(id, name, description, pointValue, null)
    {
    }

    public Achievement(string id, string name, string description, int pointValue, Badge badge)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.pointValue = pointValue;
        this.badge = badge;
    }

}
