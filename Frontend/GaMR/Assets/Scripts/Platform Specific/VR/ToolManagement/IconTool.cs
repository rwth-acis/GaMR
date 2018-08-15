using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconTool : Tool
{

    public Texture2D trackpadIcon;
    public Vector3 iconPosition = new Vector3(0, 0, 0.003f);
    public Vector3 iconEulerAngles = new Vector3(0, 180, 90);
    public Vector3 iconScale = new Vector3(0.002f, 0.002f, 0.002f);

    private GameObject trackpadIconInstance;

    private void InstantiateTrackpadIcon ()
    {
        if (trackpadIcon != null && trackpadIconInstance == null)
        {
            Transform trackpadHook = transform.Find("Model/trackpad/attach");
            if (trackpadHook != null)
            {
                trackpadIconInstance = new GameObject("Trackpad Icon");
                SpriteRenderer spriteRenderer = trackpadIconInstance.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = Sprite.Create(trackpadIcon, new Rect(0.0f, 0.0f, trackpadIcon.width, trackpadIcon.height), new Vector2(0.5f, 0.5f), 100.0f);
                trackpadIconInstance.transform.parent = trackpadHook;
                trackpadIconInstance.transform.localPosition = iconPosition;
                trackpadIconInstance.transform.localRotation = Quaternion.Euler(iconEulerAngles);
                trackpadIconInstance.transform.localScale = iconScale;
            }
        }
    }

    protected override void OnEnable ()
    {
        base.OnEnable();
        InstantiateTrackpadIcon();
        if (trackpadIconInstance != null)
        {
            trackpadIconInstance.SetActive(true);
        }
    }

    protected override void OnDisable ()
    {
        base.OnDisable();
        if (trackpadIconInstance != null)
        {
            trackpadIconInstance.SetActive(false);
        }
    }

}
