using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TextureLoader : MonoBehaviour
{

    Renderer rend;
    public string textureUrl;
    private InformationManager infoManager;
    public string modelName;

    // Use this for initialization
    void Start()
    {
        infoManager = ComponentGetter.GetComponentOnGameobject<InformationManager>("InformationManager");
        RestManager restManager = ComponentGetter.GetComponentOnGameobject<RestManager>("RestManager");
        rend = GetComponent<Renderer>();
        restManager.GetTexture(infoManager.BackendAddress + "/resources/texture/" + modelName + "/" + textureUrl, OnFinished);
    }

    private void OnFinished(Texture requestResult)
    {
        rend.material.SetTexture("_MainTex", requestResult);
    }
}
