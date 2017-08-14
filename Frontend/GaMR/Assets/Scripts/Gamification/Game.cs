﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game
{
    private string id;
    private string description;
    private string commtype;
    private bool useCommtype;

    public string ID { get { return id; } }
    public string Description { get { return description; } }
    public string CommType { get { return commtype; } }
    public bool UseCommType { get {return useCommtype; } }

    public Game(string id) : this(id, "")
    {

    }

    public Game(string id, string description)
    {
        this.useCommtype = false;
        InitializeParameters(id, description, commtype);
    }

    public Game(string id, string description, string commtype)
    {
        this.useCommtype = true;
        InitializeParameters(id, description, commtype);
    }

    private void InitializeParameters(string id, string description, string commtype)
    {
        this.id = id;
        this.description = description;
        this.commtype = commtype;
    }

}