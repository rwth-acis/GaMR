using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GamificationFramework : Singleton<GamificationFramework> {

	public void CreateGame(string gameName)
    {
        if (gameName == "")
        {
            Debug.LogWarning("Tried to create a game without a name");
            return;
        }
        // the first letter of the game needs to be lower-case
        gameName = gameName[0].ToString().ToLower() + gameName.Substring(1, gameName.Length - 1);

        WWWForm body = new WWWForm();
        body.AddField("gameid", gameName);
        RestManager.Instance.POST(InformationManager.Instance.IPAddressGamification + ":8086/gamification/games/data", body, OperationFinished);
    }

    public void ValidateLogin(Action<UnityWebRequest> callback)
    {
        RestManager.Instance.POST(InformationManager.Instance.IPAddressGamification + ":8086/gamification/games/validation", callback);
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
        CreateGame("testgame");
    }
}
