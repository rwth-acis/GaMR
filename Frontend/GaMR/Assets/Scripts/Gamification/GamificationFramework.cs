using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GamificationFramework : Singleton<GamificationFramework>
{

    // ---------------------------------------------------------------
    // Games
    // ---------------------------------------------------------------

    public void CreateGame(Game game)
    {
        if (game.ID == "")
        {
            Debug.LogWarning("Tried to create a game without an id");
            return;
        }

        WWWForm body = game.ToWWWForm();
        RestManager.Instance.POST(InformationManager.Instance.GamificationAddress + "/gamification/games/data", body, OperationFinished);
    }

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

    private void ConvertGameDetailsToGame(string json, object[] args)
    {
        Game game = null;
        if (json != null)
        {
            game = Game.FromJson(json);
        }
        Action<Game> secondaryCallback = ((Action<Game>)args[0]);
        if (secondaryCallback != null)
        {
            secondaryCallback(game);
        }
    }

    public void ValidateLogin(Action<UnityWebRequest> callback)
    {
        RestManager.Instance.POST(InformationManager.Instance.GamificationAddress + "/gamification/games/validation", callback);
    }

    // ---------------------------------------------------------------
    // Achievements
    // ---------------------------------------------------------------

    public void CreateAchievement(Game game, Achievement achievement)
    {
        WWWForm body = achievement.ToWWWForm();

        RestManager.Instance.POST(InformationManager.Instance.GamificationAddress + "/gamification/achievements/" + game.ID, body, OperationFinished);
    }

    // ---------------------------------------------------------------
    // Actions
    // ---------------------------------------------------------------

    public void CreateAction(Game game, GameAction action)
    {
        WWWForm body = action.ToWWWForm();

        RestManager.Instance.POST(InformationManager.Instance.GamificationAddress + "/gamification/actions/" + game.ID, body, OperationFinished);
    }

    public void UpdateAction(Game game, GameAction action)
    {
        WWWForm body = action.ToWWWForm();

        RestManager.Instance.PUT(InformationManager.Instance.GamificationAddress + "/gamification/actions/" + game.ID + "/" + action.ID, body, OperationFinished);
    }

    public void TriggerAction()
    {

    }

    // ---------------------------------------------------------------
    // Badges
    // ---------------------------------------------------------------

    public void CreateBadge(Game game, Badge badge)
    {
        WWWForm body = badge.ToWWWForm();

        RestManager.Instance.POST(InformationManager.Instance.GamificationAddress + "/gamification/badges/" + game.ID, body, OperationFinished);
    }

    // ---------------------------------------------------------------
    // Points
    // ---------------------------------------------------------------

    public void ChangePointUnitName(Game game, string newUnitName)
    {
        RestManager.Instance.PUT(InformationManager.Instance.GamificationAddress + "/gamification/points/" + game.ID + "/" + newUnitName, OperationFinished);
    }

    public void GetPointUnitName(Game game)
    {

    }

    // -------------------------------------------------------------------------------
    // Helper methods

    private void OperationFinished(UnityWebRequest req)
    {
        if (req.responseCode != 200)
        {
            Debug.Log("Error code for request: " + req.responseCode + " " + req.error);
        }
    }

    private void Start()
    {
        // for testing:
        GetGameDetails("testgame", Called);
    }

    private void Called(Game game)
    {
        Debug.Log(game.ID);
    }
}
