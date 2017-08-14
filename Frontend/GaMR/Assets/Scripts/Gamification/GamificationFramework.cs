using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GamificationFramework : Singleton<GamificationFramework>
{

    public void CreateGame(Game game)
    {
        if (game.ID == "")
        {
            Debug.LogWarning("Tried to create a game without a name");
            return;
        }

        WWWForm body = new WWWForm();
        body.AddField("gameid", game.ID);
        body.AddField("gamedesc", game.Description);
        if (game.UseCommType)
        {
            body.AddField("commtype", game.CommType);
        }
        RestManager.Instance.POST(InformationManager.Instance.GamificationAddress + "/gamification/games/data", body, OperationFinished);
    }

    public void ValidateLogin(Action<UnityWebRequest> callback)
    {
        RestManager.Instance.POST(InformationManager.Instance.GamificationAddress + "/gamification/games/validation", callback);
    }

    public void CreateAchievement(Game game, Achievement achievement)
    {
        WWWForm body = new WWWForm();
        body.AddField("achievementid", achievement.ID);
        body.AddField("achievementname", achievement.Name);
        body.AddField("achievementdesc", achievement.Description);
        body.AddField("achievementpointvalue", achievement.PointValue);
        body.AddField("achievementbdageid", achievement.Badge.ID);
        if (achievement.NotificationCheck)
        {
            // if the field exists, the bool variable will be set to true in the framework (no matter which value the www-field has)
            body.AddField("achievementnotificationcheck", "true");
        }
        body.AddField("achievementnotificationmessage", achievement.NotificationMessage);

        RestManager.Instance.POST(InformationManager.Instance.GamificationAddress + "/gamification/achievements/" + game.ID, body, OperationFinished);
    }

    public void CreateAction(Game game, GameAction action)
    {
        WWWForm body = new WWWForm();
        body.AddField("actionid", action.ID);
        body.AddField("actionname", action.Name);
        body.AddField("actiondesc", action.Description);
        body.AddField("actionpointvalue", action.PointValue);
        if (action.NotificationCheck)
        {
            // if the field exists, the bool variable will be set to true in the framework (no matter which value the www-field has)
            body.AddField("actionnotificationcheck", "true");
        }
        body.AddField("actionnotificationmessage", action.NotificationMessage);

        RestManager.Instance.POST(InformationManager.Instance.GamificationAddress + "/gamification/actions/" + game.ID, body, OperationFinished)
    }

    public void CreateBadge(Game game, Badge badge)
    {
        WWWForm body = new WWWForm();
        body.AddField("badgeid", badge.ID);
        body.AddField("badgename", badge.Name);
        body.AddField("badgedesc", badge.Description);
        if (badge.NotificationCheck)
        {
            // if the field exists, the bool variable will be set to true in the framework (no matter which value the www-field has)
            body.AddField("badgenotificationcheck", "true");
        }
        body.AddField("badgenotificationmessage", badge.NotificationMessage);
        body.AddBinaryData("badgeimageinput", badge.Image.GetRawTextureData());
    }

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
        Game game = new Game("testgame");
        CreateGame(game);
    }
}
