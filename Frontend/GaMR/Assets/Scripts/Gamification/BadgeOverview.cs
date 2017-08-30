using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BadgeOverview : Singleton<BadgeOverview>
{

    private List<GameObject> instantiatedBadges;
    private Dictionary<string, List<Badge>> badgesFromAllGames;
    private List<BadgeManager> badgeManagers;
    private Vector3 instantiatePosition;

    private void Start()
    {
        instantiatedBadges = new List<GameObject>();
        badgeManagers = new List<BadgeManager>();
        badgesFromAllGames = new Dictionary<string, List<Badge>>();
        instantiatePosition = new Vector3(0, 0, 2);
    }

    public void GenerateOverview()
    {
        instantiatePosition = InFrontOfCamera.Instance.Position;

        GamificationFramework.Instance.GetSeparateGameInfos(
            (games, resCode) =>
            {
                if (resCode == 200)
                {
                    for (int i = 0; i < games.Length; i++)
                    {
                        int indexForLambda = i;
                        // only look at this game if the member is actually added to the game
                        if (games[i].memberHas)
                        {
                            // get the badges of the game
                            GamificationFramework.Instance.GetBadgesOfUser(games[i].game_id,
                                (badgeArray, badgesCode) =>
                                {
                                    if (badgesCode == 200)
                                    {
                                        if (badgesFromAllGames.ContainsKey(games[indexForLambda].game_id))
                                        {
                                            badgesFromAllGames[games[indexForLambda].game_id].AddRange(badgeArray);
                                        }
                                        else
                                        {
                                            List<Badge> badgesOfGame = new List<Badge>();
                                            badgesOfGame.AddRange(badgeArray);
                                            badgesFromAllGames.Add(games[indexForLambda].game_id, badgesOfGame);
                                            ShowBadges(games[indexForLambda].game_id);
                                        }
                                    }
                                }
                                );
                        }
                    }
                }
            }
            );
    }


    private void ShowBadges(string gameId)
    {
        List<Badge> badgeList = badgesFromAllGames[gameId];
        for (int i=0;i<badgeList.Count;i++)
        {
            GameObject badgeInstance = (GameObject) Instantiate(Resources.Load("Badge"));
            badgeInstance.transform.position = instantiatePosition;
            instantiatePosition += new Vector3(0.5f, 0, 0);
            instantiatedBadges.Add(badgeInstance);
            BadgeManager manager = badgeInstance.GetComponent<BadgeManager>();
            DownloadImage(gameId, badgeList[i], manager);
        }
    }

    private void DownloadImage(string gameId, Badge badge, BadgeManager manager)
    {
        GamificationFramework.Instance.GetBadgeImage(gameId, badge.ID,
                                    (texture, resImageCode) =>
                                    {
                                        Debug.Log("Badge image loaded");
                                        if (resImageCode == 200)
                                        {
                                            badge.Image = (Texture2D)texture;
                                            manager.Badge = badge;
                                        }
                                    }
                                    );
    }

    public void HideBadges()
    {

    }
}
