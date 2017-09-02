using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;

public class Thumbnail : MonoBehaviour
{
    private Renderer rend;
    private TextMesh textMesh;
    private bool visible;
    private GameObject frameObject;

    private string modelName;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        textMesh = transform.Find("Caption").GetComponent<TextMesh>();
        frameObject = transform.Find("Thumbnail Frame").gameObject;
        FocusableButton btn = gameObject.AddComponent<FocusableButton>();
        btn.FocusHighlight = frameObject;
        btn.OnPressed = OnClicked;
    }

    private void OnClicked()
    {
        Debug.Log("Pressed " + modelName);
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
        textMesh.text = modelName;
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
