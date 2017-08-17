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

        List<IMultipartFormSection> body = game.ToMultipartFormData();
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

    public void ValidateLogin(Action<UnityWebRequest> callback)
    {
        RestManager.Instance.POST(InformationManager.Instance.GamificationAddress + "/gamification/games/validation", callback);
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

    public void TriggerAction()
    {

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

    public void GetPointUnitName(string gameId, Action<string> callback)
    {
        object[] passOnArgs = { callback };
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
        if (req.responseCode != 200)
        {
            Debug.Log("Error code for request: " + req.responseCode + " " + req.error);
            Debug.Log("Error: " + req.downloadHandler.text);
        }
        else
        {
            Debug.Log(req.downloadHandler.text);
        }
    }

    private void Start()
    {
        // for testing:
        GetPointUnitName("testgame", Result);
    }

    private void Result(string obj)
    {
        Debug.Log(obj);
    }
}
