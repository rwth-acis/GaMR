using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// If the component is attached to a gameobject it downloads the specified texture
/// when finished it applies the texture to the main material of the object
/// </summary>
public class TextureLoader : MonoBehaviour
{

    Renderer rend;
    public string textureUrl;
    private InformationManager infoManager;
    public string modelName;

    /// <summary>
    /// Called when the component is instantiated
    /// Gets the necessary components and downloads the texture specified in textureUrl and modelName
    /// </summary>
    void Start()
    {
        infoManager = ComponentGetter.GetComponentOnGameobject<InformationManager>("InformationManager");
        RestManager restManager = ComponentGetter.GetComponentOnGameobject<RestManager>("RestManager");
        rend = GetComponent<Renderer>();
        WaitCursor.Show();
        restManager.GetTexture(infoManager.BackendAddress + "/resources/texture/" + modelName + "/" + textureUrl, OnFinished);
    }

    /// <summary>
    /// Called when the texture has finished downloading
    /// Applies the texture to the main material
    /// </summary>
    /// <param name="requestResult">The downloaded texture</param>
    private void OnFinished(UnityWebRequest req, Texture requestResult)
    {
        WaitCursor.Hide();
        if (req.responseCode != 200)
        {
            MessageBox.Show(LocalizationManager.Instance.ResolveString("Could not fetch texture"), MessageBoxType.ERROR);
        }
        rend.material.SetTexture("_MainTex", requestResult);
    }
}
