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
        // Example to get audio file ========================================
        //UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip("http://localhost:8080/resources/annotation/audio/load/brain/horse", AudioType.OGGVORBIS);
        //yield return req.Send();

        //Debug.Log(req.responseCode);
        //AudioClip clip = DownloadHandlerAudioClip.GetContent(req);
        //Debug.Log(clip.length);

        // Example to get bytes =============================================
        UnityWebRequest req = UnityWebRequest.Get("http://localhost:8080/resources/annotation/audio/load/brain/test");
        yield return req.Send();
        byte[] reqBytes = req.downloadHandler.data;
        float[] reqFloats = new float[reqBytes.Length / 4];
        for (int i = 0; i < reqBytes.Length; i += 4)
        {
            reqFloats[i / 4] = BitConverter.ToSingle(reqBytes, i);
        }

        AudioClip clip = AudioClip.Create("", reqFloats.Length, 2, 44100, false);
        clip.SetData(reqFloats, 0);

        Debug.Log(clip.length);

        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.spatialBlend = 1;
        source.clip = clip;
        source.Play();

        // Post data =========================================================
        //UnityWebRequest post = new UnityWebRequest("http://localhost:8080/resources/annotation/audio/save/brain/test", "POST");

        //float[] clipSamples = new float[clip.samples];
        //clip.GetData(clipSamples, 0);

        //List<byte> byteSamples = new List<byte>();
        //for (int i=0;i<clipSamples.Length;i++)
        //{
        //    byteSamples.AddRange(BitConverter.GetBytes(clipSamples[i]));
        //}

        //UnityWebRequest post = new UnityWebRequest("http://localhost:8080/resources/annotation/audio/save/brain/test", "POST");
        //post.uploadHandler = new UploadHandlerRaw(byteSamples.ToArray());
        //yield return post.Send();

        // Example for audio recording =========================================

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
