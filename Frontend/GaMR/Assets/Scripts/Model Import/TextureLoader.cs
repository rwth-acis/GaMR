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

    // Use this for initialization
    void Start()
    {
        infoManager = GameObject.Find("InformationManager").GetComponent<InformationManager>();
        rend = GetComponent<Renderer>();
        RestManager restFactory = GameObject.Find("RestManager").GetComponent<RestManager>();
        restFactory.GetTexture(infoManager.BackendAddress + "/resources/texture/" + textureUrl, OnFinished);
    }

    private void OnFinished(Texture requestResult)
    {
        rend.material.SetTexture("_MainTex", requestResult);
    }
}
