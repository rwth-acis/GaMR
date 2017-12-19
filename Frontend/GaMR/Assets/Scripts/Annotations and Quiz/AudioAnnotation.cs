using System;
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
        UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip("http://localhost:8080/resources/annotation/audio/load/brain/test", AudioType.OGGVORBIS);
        yield return req.Send();

        Debug.Log(req.responseCode);
        AudioClip clip = DownloadHandlerAudioClip.GetContent(req);
        Debug.Log(clip.length);


        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.spatialBlend = 1;
        source.clip = clip;
        source.Play();

        //UnityWebRequest post = new UnityWebRequest("http://localhost:8080/resources/annotation/audio/save/brain/test", "POST");
        


        // Example for audio recording
        //AudioClip recording = Microphone.Start(null, false, 60, 44100);
        //yield return new WaitForSeconds(10);
        //int endTime = Microphone.GetPosition(null);
        //Microphone.End(null);
        //float[] audioSamples = new float[recording.samples];
        //recording.GetData(audioSamples, 0);
        //float[] cutSamples = new float[endTime];
        //Array.Copy(audioSamples, cutSamples, cutSamples.Length);
        //AudioClip cutClip = AudioClip.Create("cutClip", cutSamples.Length, 1, 44100, false);

        //cutClip.SetData(cutSamples, 0);

        //Debug.Log(cutClip.length);
    }
    
}
