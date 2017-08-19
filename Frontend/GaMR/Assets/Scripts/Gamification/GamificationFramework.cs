using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Contains the wrapper methods to access the API of the Gamfication Framework
/// </summary>
public class GamificationFramework : Singleton<GamificationFramework>
{

    // ---------------------------------------------------------------
    // Games
    // ---------------------------------------------------------------

    /// <summary>
    /// Create a new game
    /// </summary>
    /// <param name="game">The game data which are passed on to the Gamification Framework</param>
    public void CreateGame(Game game)
    {
        if (game.ID == "")
        {
            Debug.LogWarning("Tried to create a game without an id");
            return;
        }

        List<IMultipartFormSection> body = game.ToMultipartFormData();
        RestManager.Instance.POST(InformationManager.Instance.GamificationAddress + "/gamification/games/data", body, OperationFinished);
    }

    /// <summary>
    /// Gets the details about a specific game. The result is passed to the callback method.
    /// </summary>
    /// <param name="gameId">The game id of the game</param>
    /// <param name="callWithResult">This method will be called with the resulting game object</param>
    public void GetGameDetails(string gameId, Action<Game> callWithResult)
    {
        if (callWithResult != null)
        {
            object[] args = { callWithResult };
            RestManager.Instance.GET(InformationManager.Instance.GamificationAddress + "/gamification/games/data/" + gameId, ConvertGameDetailsToGame, args);
        }
        else
        {
            Debug.LogWarning("Getting game details without providing callWithResult makes no sense");
        }
    }

    public void DeleteGame(string gameId)
    {
        RestManager.Instance.DELETE(InformationManager.Instance.GamificationAddress + "/gamification/games/data/" + gameId, OperationFinished);
    }

    /// <summary>
    /// Called when the GetGameDetails operation is finished. Checks if the request was successful and invokes the secondary callback.
    /// </summary>
    /// <param name="result">The result of the web request</param>
    /// <param name="args">Additional arguments which have been passed through the request. This should contain a secondary callback method.</param>
    private void ConvertGameDetailsToGame(UnityWebRequest result, object[] args)
    {
        Game game = null;
        if (result.responseCode == 200)
        {
            game = Game.FromJson(result.downloadHandler.text);
        }
        else
        {
            MessageBox.Show(LocalizationManager.Instance.ResolveString("Could not fetch the game details"), MessageBoxType.ERROR);
        }
        Action<Game> secondaryCallback = ((Action<Game>)args[0]);
        if (secondaryCallback != null)
        {
            secondaryCallback(game);
        }
    }

    /// <summary>
    /// Validates the login credentials of the user
    /// </summary>
    /// <param name="callback">Method which gets called when the request is finished. The parameter of the method will contain the request result.</param>
    public void ValidateLogin(Action<UnityWebRequest> callback)
    {
        RestManager.Instance.POST(InformationManager.Instance.GamificationAddress + "/gamification/games/validation", callback);
    }

    // ---------------------------------------------------------------
    // Quests
    // ---------------------------------------------------------------

    /// <summary>
    /// Creates a new quest
    /// </summary>
    /// <param name="gameId">The game id of the game that should contain the quest</param>
    /// <param name="quest">The quest data to pass to the Gamification Framework</param>
    public void CreateQuest(string gameId, Quest quest)
    {
        RestManager.Instance.POST(InformationManager.Instance.GamificationAddress + "/gamification/quests/" + gameId, quest.ToJson(), OperationFinished);
    }

    // ---------------------------------------------------------------
    // Achievements
    // ---------------------------------------------------------------

    public void CreateAchievement(string gameId, Achievement achievement)
    {
        List<IMultipartFormSection> body = achievement.ToMultipartFormData();
        RestManager.Instance.POST(InformationManager.Instance.GamificationAddress + "/gamification/achievements/" + gameId, body, OperationFinished);
    }

    // ---------------------------------------------------------------
    // Actions
    // ---------------------------------------------------------------

    public void CreateAction(string gameId, GameAction action)
    {
        List<IMultipartFormSection> body = action.ToMultipartFormData();
        RestManager.Instance.POST(InformationManager.Instance.GamificationAddress + "/gamification/actions/" + gameId, body, OperationFinished);
    }

    public void UpdateAction(string gameId, GameAction action)
    {
        List<IMultipartFormSection> body = action.ToMultipartFormData();
        RestManager.Instance.PUT(InformationManager.Instance.GamificationAddress + "/gamification/actions/" + gameId + "/" + action.ID, body, OperationFinished);
    }

    public void TriggerAction(string gameId, string actionId)
    {
        RestManager.Instance.POST(InformationManager.Instance.GamificationAddress
            + "/visualization/actions/" + gameId + "/" + actionId + "/" + InformationManager.Instance.UserInfo.preferred_username,
            OperationFinished);
    }

    // ---------------------------------------------------------------
    // Badges
    // ---------------------------------------------------------------

    public void CreateBadge(string gameId, Badge badge)
    {
        //WWWForm body = badge.ToWWWForm();
        //RestManager.Instance.POST(InformationManager.Instance.GamificationAddress + "/gamification/badges/" + gameId, body, OperationFinished);
    }

    // ---------------------------------------------------------------
    // Points
    // ---------------------------------------------------------------

    public void ChangePointUnitName(string gameId, string newUnitName)
    {
        RestManager.Instance.PUT(InformationManager.Instance.GamificationAddress + "/gamification/points/" + gameId + "/name/" + newUnitName, OperationFinished);
    }

    public void GetPointUnitName(string gameId, Action<string> callWithResult)
    {
        object[] passOnArgs = { callWithResult };
        RestManager.Instance.GET(InformationManager.Instance.GamificationAddress + "/gamification/points/" + gameId + "/name", EvaluateGetPointUnitName, passOnArgs);
    }

    private void EvaluateGetPointUnitName(UnityWebRequest req, object[] passOnArgs)
    {
        if (req.responseCode != 200)
        {
            Debug.Log("Error code for request: " + req.responseCode + " " + req.error);
            Debug.Log("Error: " + req.downloadHandler.text);
        }
        else
        {
            if (passOnArgs != null && passOnArgs.Length > 0)
            {
                JsonPointUnit unit = JsonUtility.FromJson<JsonPointUnit>(req.downloadHandler.text);
                ((Action<string>)passOnArgs[0])(unit.pointUnitName);
            }
        }
    }

    // -------------------------------------------------------------------------------
    // Helper methods

    private void OperationFinished(UnityWebRequest req)
    {
        if (req.responseCode != 200 || req.responseCode != 201)
        {
            Debug.Log("Error code for request: " + req.responseCode + " " + req.error);
            Debug.Log("Error: " + req.downloadHandler.text);
        }
        else
        {
            Debug.Log("Success");
            Debug.Log(req.downloadHandler.text);
        }
    }

    private void Start()
    {
        // for testing:
        Achievement achievement = new Achievement("testachievement", "testachievement", "testdescr", 5);
        GameAction action = new GameAction("testaction2", "testaction2", "a test action", 2);
        Quest quest = new Quest("testquest3", "testquest3", QuestStatus.REVEALED, achievement, false, false, 0, "a quest description");
        quest.AddAction(action, 2);

        CreateQuest("testgame2", quest);
    }

    private void Result(string obj)
    {
        Debug.Log(obj);
    }
}
