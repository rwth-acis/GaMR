using HoloToolkit.Sharing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingBoxId
{
    public long UserId;
    public int BoxId;

    private static int counter = 0;

    public override string ToString()
    {
        return UserId.ToString("X16") + BoxId.ToString("X8");
    }

    public BoundingBoxId()
    {
        UserId = SharingStage.Instance.Manager.GetLocalUser().GetID();
        BoxId = counter;
        counter++;
    }

    public BoundingBoxId(long userId, int boxId)
    {
        UserId = userId;
        BoxId = boxId;
    }

    public bool IsLocal()
    {
        return UserId == SharingStage.Instance.Manager.GetLocalUser().GetID();
    }
}
