using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Stores all data for a gamification game
/// </summary>
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
        if (id.Length > 0)
        {
            id = id[0].ToString().ToLower() + id.Substring(1);
        }
        this.description = description;
        this.commtype = commtype;
    }

    /// <summary>
    /// Converts the game object to multipart/form-data for POST and PUT queries
    /// It is designed to be compatible with the Gamification Framework
    /// </summary>
    /// <returns>The multipart/form-data with the filled fields</returns>
    public List<IMultipartFormSection> ToMultipartFormData()
    {
        List<IMultipartFormSection> body = new List<IMultipartFormSection>();
        body.Add(new MultipartFormDataSection("gameid", ID));
        if (Description != "")
        {
            body.Add(new MultipartFormDataSection("gamedesc", Description));
        }
        if (UseCommType && CommType != null && CommType != "")
        {
            body.Add(new MultipartFormDataSection("commtype", CommType));
        }

        return body;
    }

    /// <summary>
    /// Parses a game from a json string
    /// </summary>
    /// <param name="json">The parsed game object</param>
    /// <returns></returns>
    public static Game FromJson(string json)
    {
        JsonGame jsonGame = JsonUtility.FromJson<JsonGame>(json);

        Game game = new Game(jsonGame.id, jsonGame.description, jsonGame.commType);
        return game;
    }
}

/// <summary>
/// A json game object
/// Used for parsing the json string of the Gamification Framework to a runtime object
/// </summary>
class JsonGame
{
    public string commType;
    public string id;
    public string description;
}