using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;

public class Thumbnail : FocusableButton
{
    private bool visible;
    private GameObject frameObject;

    private string modelName;

    public ThumbnailInstantiation InstantiationParent
    {
        get; set;
    }

    private void Start()
    {
        OnPressed = OnClicked;
    }

    private void OnClicked()
    {
        Debug.Log("Pressed " + modelName);
        if (InstantiationParent != null)
        {
            InstantiationParent.OnThumbnailClicked(modelName);
        }
    }

    public bool Visible
    {
        get { return visible; }
        set
        {
            visible = value;
            gameObject.SetActive(value);
        }
    }

    public void LoadImage(string modelName)
    {
        this.modelName = modelName;
        Text = modelName;
        rend.material.mainTexture = null;
        RestManager.Instance.GET(InformationManager.Instance.FullBackendAddress + "/resources/model/" + modelName + "/thumbnail",
            reqRes =>
            {
                if (reqRes.responseCode == 200)
                {
                    Texture2D tex = new Texture2D(20, 20);
                    if (tex.LoadImage(reqRes.downloadHandler.data))
                    {
                        rend.material.mainTexture = tex;
                        return;
                    }
                }
                // else:
                // just show the model name
            }
            );
    }
}
