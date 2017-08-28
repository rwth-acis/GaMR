using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamificationManager : MonoBehaviour
{
    private Badge badge;
    private BadgeManager badgeManager;

    public string gameId { get; set; }

    public Quest Quest { get; set; }

    public Achievement AchievementOfQuest { get; set; }

    public Badge Badge
    {
        get
        {
            return badge;
        }
        set
        {
            badge = value;
            if (BadgeManager != null)
            {
                BadgeManager.Badge = badge;
            }
            AchievementOfQuest.BadgeId = Badge.ID;

            GamificationFramework.Instance.UpdateAchievement(gameId, AchievementOfQuest,
                (resAchievement, resCode) =>
                {
                    if (resCode != 200)
                    {
                        Debug.Log("Could not update the achievement");
                    }
                }
                );
        }
    }

    public BadgeManager BadgeManager
    {
        get
        {
            return badgeManager;
        }
        set
        {
            badgeManager = value;
            if (badgeManager != null)
            {
                badgeManager.Badge = badge;
            }
        }
    }

    public void CommitQuest()
    {
        GamificationFramework.Instance.UpdateQuest(gameId, Quest);
    }
}
