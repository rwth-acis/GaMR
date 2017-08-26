using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamificationManager : MonoBehaviour
{
    private Badge badge;

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
        }
    }

    public BadgeManager BadgeManager { get; set; }

    public void Commit()
    {
        GamificationFramework.Instance.UpdateQuest(gameId, Quest);
    }
}
