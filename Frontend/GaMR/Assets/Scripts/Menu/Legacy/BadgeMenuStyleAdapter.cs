using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadgeMenuStyleAdapter : MenuStyleAdapter
{
    Material texMat;

    public override void Initialize()
    {
        texMat = transform.Find("Badge").GetComponent<Renderer>().materials[1];
        clickListeners = new List<System.Action>();
    }

    public override void UpdateIcon(Texture newIcon)
    {
        texMat.mainTexture = newIcon;
    }

    public override void OnFocusEnter()
    {
    }

    public override void OnFocusExit()
    {
    }
}
