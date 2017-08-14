using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Quest
{
    [SerializeField]
    private string questid;
    [SerializeField]
    private string questname;
    private string questStatus;
    private string questachievementid;
    private bool questquestflag;
    private bool questpointflag;
    private int questpointvalue;
    private List<GameAction> questactionids;
    private string questdescription;
    private bool questnotificationcheck;
    private string questnotificationmessage;
}
