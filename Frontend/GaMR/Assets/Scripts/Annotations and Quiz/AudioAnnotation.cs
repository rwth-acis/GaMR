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
        UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip("http://localhost:8080/resources/annotation/audio/load/brain/horse", AudioType.OGGVORBIS);
        yield return req.Send();

        Debug.Log(req.responseCode);
        AudioClip clip = DownloadHandlerAudioClip.GetContent(req);
        Debug.Log(clip.length);


        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.spatialBlend = 1;
        source.clip = clip;
        source.Play();

        //UnityWebRequest post = new UnityWebRequest("http://localhost:8080/resources/annotation/audio/save/brain/test", "POST");

        //float[] clipSamples = new float[clip.samples];

        //clip.GetData(clipSamples, 0);

        //Int16[] intData = new Int16[clipSamples.Length];
        //byte[] byteData = new byte[clipSamples.Length * 2];

        //int rescaleFactor = 32767;

        //for (int i=0;i<clipSamples.Length;i++)
        //{
        //    intData[i] = (short)(clipSamples[i] * rescaleFactor);
        //    byte[] byteConversion = new byte[2];
        //    byteConversion = BitConverter.GetBytes(intData[i]);
        //    byteConversion.CopyTo(byteData, i * 2);
        //}

        //List<byte> completeWAV = new List<byte>();
        //completeWAV.AddRange(System.Text.Encoding.UTF8.GetBytes("RIFF"));
        //completeWAV.AddRange(BitConverter.GetBytes(byteData.Length - 8));
        //completeWAV.AddRange(System.Text.Encoding.UTF8.GetBytes("WAVE"));
        //completeWAV.AddRange(System.Text.Encoding.UTF8.GetBytes("fmt "));
        //completeWAV.AddRange(BitConverter.GetBytes(16));
        //completeWAV.AddRange(BitConverter.GetBytes((UInt16)1));
        //completeWAV.AddRange(BitConverter.GetBytes(clip.channels));
        //completeWAV.AddRange(BitConverter.GetBytes(clip.frequency));
        //completeWAV.AddRange(BitConverter.GetBytes(clip.frequency * clip.channels * 2));
        //completeWAV.AddRange(BitConverter.GetBytes((ushort)clip.channels * 2));
        //completeWAV.AddRange(BitConverter.GetBytes((UInt16)16));
        //completeWAV.AddRange(System.Text.Encoding.UTF8.GetBytes("data"));
        //completeWAV.AddRange(BitConverter.GetBytes(clip.samples * clip.channels * 2));
        //completeWAV.AddRange(byteData);

        //UnityWebRequest post = new UnityWebRequest("http://localhost:8080/resources/annotation/audio/save/brain/test", "POST");
        //post.uploadHandler = new UploadHandlerRaw(completeWAV.ToArray());
        //yield return post.Send();


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
