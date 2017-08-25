using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamificationManager : MonoBehaviour
{
    public string gameId{ get; set; }

    public Quest Quest { get; set; }

    public Achievement AchievementOfQuest { get; set; }

    public Badge Badge { get; set; }

    public void Commit()
    {
        GamificationFramework.Instance.UpdateQuest(gameId, Quest);
    }
}
