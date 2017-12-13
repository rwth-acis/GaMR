using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AudioAnnotation : MonoBehaviour
{

    // Update is called once per frame
    void Start()
    {
        StartCoroutine(SendRequest());
    }

    private IEnumerator SendRequest()
    {
        UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip("http://localhost:8080/resources/annotation/audio/load/brain/horse", AudioType.OGGVORBIS);
        yield return req.Send();

        Debug.Log(req.responseCode);
        AudioClip clip = DownloadHandlerAudioClip.GetContent(req);
        Debug.Log(clip.length);
    }
}
