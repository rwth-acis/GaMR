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
    public void CreateGame(Game game, Action<Game, long> callback)
    {
        if (game.ID == "")
        {
            Debug.LogWarning("Tried to create a game without an id");
            return;
        }

        List<IMultipartFormSection> body = game.ToMultipartFormData();
        RestManager.Instance.POST(InformationManager.Instance.GamificationAddress + "/gamification/games/data", body, reqRes =>
        {
            if (callback != null)
            {
                if (reqRes.responseCode == 200 || reqRes.responseCode == 201)
                {
                    callback(game, reqRes.responseCode);
                }
                else

                {
                    callback(null, reqRes.responseCode);
                }
            }
        });

        CreateAction(game.ID, new GameAction("pseudoaction", "a pseudo action", "pseudo action for all quests to avoid empty lists", 0), null);
    }

    /// <summary>
    /// Gets the details about a specific game. The result is passed to the callback method.
    /// </summary>
    /// <param name="gameId">The game id of the game</param>
    /// <param name="callWithResult">This method will be called with the resulting game object and the response code of the request</param>
    public void GetGameDetails(string gameId, Action<Game, long> callWithResult)
    {
        if (callWithResult != null)
        {
            object[] args = { callWithResult };
            RestManager.Instance.GET(InformationManager.Instance.GamificationAddress + "/gamification/games/data/" + gameId, reqRes =>
            {
                Game game = null;
                if (reqRes.responseCode == 200)
                {
                    game = Game.FromJson(reqRes.downloadHandler.text);
                }
                if (callWithResult != null)
                {
                    callWithResult(game, reqRes.responseCode);
                }
            });
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

    public void AddUserToGame(string gameId, Action<long> callback)
    {
        RestManager.Instance.POST(InformationManager.Instance.GamificationAddress + "/gamification/games/data/" + gameId + "/" + InformationManager.Instance.UserInfo.preferred_username,
            reqRes =>
            {
                if (callback != null)
                {
                    callback(reqRes.responseCode);
                }
            }
            );
    }

    public void RemoveUserFromGame(string gameId, Action<long> callback)
    {
        RestManager.Instance.DELETE(InformationManager.Instance.GamificationAddress + "/gamification/games/data" + gameId + "/" + InformationManager.Instance.UserInfo.preferred_username,
            reqRes =>
            {
                if (callback != null)
                {
                    callback(reqRes.responseCode);
                }
            }
            );
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
    public void CreateQuest(string gameId, Quest quest, Action<Quest, long> callback)
    {
        RestManager.Instance.POST(InformationManager.Instance.GamificationAddress + "/gamification/quests/" + gameId, quest.ToJson(),
            reqRes =>
            {
                if (reqRes.responseCode == 201)
                {
                    Debug.Log("Created Quest " + quest.ID + " (" + gameId + ")");
                }

                if (callback != null)
                {
                    callback(quest, reqRes.responseCode);
                }
            }
            );
    }

    public void UpdateQuest(string gameId, Quest quest)
    {
        RestManager.Instance.PUT(InformationManager.Instance.GamificationAddress + "/gamification/quests/" + gameId, quest.ToJson(), OperationFinished);
    }

    public void GetOrCreateQuest(string gameId, Quest quest, Action<Quest> callback)
    {
        GetQuestWithId(gameId, quest.ID,
            (resQuest, resCode) =>
            {
                if (resCode == 200)
                {
                    if (callback != null)
                    {
                        callback(resQuest);
                    }
                }
                else
                {
                    CreateQuest(gameId, quest,
                        (createdQuest, createCode) =>
                        {
                            if (createCode == 200 || createCode == 201)
                            {
                                if (callback != null)
                                {
                                    callback(createdQuest);
                                }
                            }
                            else
                            {
                                callback(null);
                            }
                        }
                        );
                }
            }
            );
    }

    public void GetQuestWithId(string gameId, string questId, Action<Quest, long> callback)
    {
        RestManager.Instance.GET(InformationManager.Instance.GamificationAddress + "/gamification/quests/" + gameId + "/" + questId,
            reqRes =>
            {
                if (callback != null)
                {
                    if (reqRes.responseCode == 200)
                    {
                        Quest quest = Quest.FromJson(reqRes.downloadHandler.text);
                        if (quest != null)
                        {
                            Debug.Log("Sucessfully loaded quest " + quest.ID);
                        }
                        callback(quest, reqRes.responseCode);
                    }
                    else
                    {
                        callback(null, reqRes.responseCode);
                    }
                }
            }
            );
    }

    // ---------------------------------------------------------------
    // Achievements
    // ---------------------------------------------------------------

    public void CreateAchievement(string gameId, Achievement achievement, Action<Achievement, long> callback)
    {
        List<IMultipartFormSection> body = achievement.ToMultipartFormData();
        RestManager.Instance.POST(InformationManager.Instance.GamificationAddress + "/gamification/achievements/" + gameId, body, 
            reqRes =>
            {
                if (callback != null)
                {
                    callback(achievement, reqRes.responseCode);
                }
            }
            );
    }

    public void UpdateAchievement(string gameId, Achievement achievement, Action<Achievement, long> callback)
    {
        List<IMultipartFormSection> body = achievement.ToMultipartFormData();
        RestManager.Instance.PUT(InformationManager.Instance.GamificationAddress + "/gamification/achievements/" + gameId + "/" + achievement.ID, body,
            reqRes =>
            {
                if (callback != null)
                {
                    callback(achievement, reqRes.responseCode);
                }
            }
            );
    }

    public void GetAchievementWithId(string gameId, string achievementId, Action<Achievement, long> callback)
    {
        RestManager.Instance.GET(InformationManager.Instance.GamificationAddress + "/gamification/achievements/" + gameId + "/" + achievementId,
            reqRes =>
            {
                if (callback != null)
                {
                    if (reqRes.responseCode == 200)
                    {
                        Achievement resAchievement = Achievement.FromJson(reqRes.downloadHandler.text);
                        callback(resAchievement, reqRes.responseCode);
                    }
                    else
                    {
                        callback(null, reqRes.responseCode);
                    }
                }
            }
            );
    }

    public void GetOrCreateAchievement(string gameId, Achievement achievement, Action<Achievement> callback)
    {
        GetAchievementWithId(gameId, achievement.ID,
            (resAchievement, resCode) =>
            {
                if (resCode == 200)
                {
                    if (callback != null)
                    {
                        callback(resAchievement);
                    }
                }
                else
                {
                    CreateAchievement(gameId, achievement,
                        (createdAchievement, createCode) =>
                        {
                            if (createCode == 200 || createCode == 201)
                            {
                                if (callback != null)
                                {
                                    callback(createdAchievement);
                                }
                            }
                            else
                            {
                                callback(null);
                            }
                        }
                        );
                }
            }
            );
    }

    // ---------------------------------------------------------------
    // Actions
    // ---------------------------------------------------------------

    public void CreateAction(string gameId, GameAction action, Action<long> callback)
    {
        List<IMultipartFormSection> body = action.ToMultipartFormData();
        RestManager.Instance.POST(InformationManager.Instance.GamificationAddress + "/gamification/actions/" + gameId, body,
            reqRes =>
            {
                if (callback != null)
                {
                    callback(reqRes.responseCode);
                }
            }
            );
    }

    public void UpdateAction(string gameId, GameAction action, Action<long> callback)
    {
        List<IMultipartFormSection> body = action.ToMultipartFormData();
        RestManager.Instance.PUT(InformationManager.Instance.GamificationAddress + "/gamification/actions/" + gameId + "/" + action.ID, body,
            reqRes =>
            {
                if (callback != null)
                {
                    callback(reqRes.responseCode);
                }
            }
            );
    }

    public void DeleteAction(string gameId, string actionId, Action<long> callback)
    {
        RestManager.Instance.DELETE(InformationManager.Instance.GamificationAddress + "/gamification/actions/" + gameId + "/" + actionId,
            reqRes =>
            {
                if (callback != null)
                {
                    callback(reqRes.responseCode);
                }
            }
            );
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
        List<IMultipartFormSection> body = badge.ToMultipartForm();

        RestManager.Instance.POST(InformationManager.Instance.GamificationAddress + "/gamification/badges/" + gameId, body, OperationFinished);
    }

    public void GetBadgeWithId(string gameId, string badgeId, Action<Badge, long> callback)
    {
        RestManager.Instance.GET(InformationManager.Instance.GamificationAddress + "/gamification/badges/" + gameId + "/" + badgeId,
            reqRes =>
            {
                if (callback != null)
                {
                    if (reqRes.responseCode == 200)
                    {
                        Badge res = Badge.FromJson(reqRes.downloadHandler.text);
                        callback(res, reqRes.responseCode);
                    }
                    else
                    {
                        callback(null, reqRes.responseCode);
                    }
                }
            }
            );
    }

    public void GetBadgeImage(string gameId, string badgeId, Action<Texture, long> callback)
    {
        RestManager.Instance.GetTexture(InformationManager.Instance.GamificationAddress + "/gamification/badges/" + gameId + "/" + badgeId + "/img",
            (reqRes, texture) =>
            {
                if (callback != null)
                {
                    callback(texture, reqRes.responseCode);
                }
            }
            );
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
        //Quest quest = new Quest("Generated Quest", "Generated Quest", QuestStatus.REVEALED, "Gamified", false, false, 0, "test description");
        //quest.AddAction("defaultAction", 1);
        //CreateQuest("Brain", quest,
        //    (resQuest, resCode) => { }
        //    );

        GetQuestWithId("Brain", "Generated Quest",
            (resQuest, code) => { }
            );
    }

    private void Result(string obj)
    {
        Debug.Log(obj);
    }
}
