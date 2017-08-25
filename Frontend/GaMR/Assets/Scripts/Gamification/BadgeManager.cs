using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BadgeManager : MonoBehaviour, IInputClickHandler
{

    private Badge badge;
    private Material mat;

    public Badge Badge
    {
        get { return badge; }
        set
        {
            badge = value;
            if (badge != null)
            {
                Mat.SetTexture(badge.Name, badge.Image);
            }
        }
    }

    private Material Mat
    {
        get
        {
            if (mat == null)
            {
                mat = GetComponent<Renderer>().materials[1];
            }
            return mat;
        }
    }

    private void Start()
    {
        if (badge == null)
        {
            if (InformationManager.Instance.playerType == PlayerType.STUDENT)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void WinBadge()
    {
        // move badge in front of the user
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        if (InformationManager.Instance.playerType != PlayerType.STUDENT)
        {
            BadgeEditor.ShowBadges();
        }
    }
}
