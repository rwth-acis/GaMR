using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HoloToolkit.Sharing.Tests;

public class TapToPlaceSynchronization : TapToPlace
{

    BoundingBoxInfo info;

    protected override void Start()
    {
        base.Start();
        info = GetComponent<BoundingBoxInfo>();
    }


    public override void OnInputClicked(InputClickedEventData eventData)
    {

        if (IsBeingPlaced) // currently still being placed => this tap is for fixing the position
        {
            CustomMessages.Instance.SendBoundingBoxTransform(info.Id, transform.localPosition, transform.localRotation, transform.localScale);
        }
        base.OnInputClicked(eventData);
    }
}
