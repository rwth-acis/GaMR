using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class RecordingManager : Singleton<RecordingManager>
{
    private List<float> recording;
    private AudioClip currentClip;
    private float currentClipTime;

    private const int recordingLength = 10;

    public bool IsRecording
    {
        get; private set;
    }

    private void Start()
    {
        StartCoroutine(TestRecording());
    }

    private IEnumerator TestRecording()
    {
        StartRecording();
        yield return new WaitForSeconds(5);
        AudioClip clip = StopRecording();
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.PlayClip(clip, false);
        byte[] oggBytes;
        SavWav.ConvertToWav(clip, out oggBytes);
        Debug.Log(oggBytes);
        UnityWebRequest post = new UnityWebRequest("http://localhost:8080/resources/annotation/audio/save/brain/test", "POST");
        post.uploadHandler = new UploadHandlerRaw(oggBytes);
        yield return post.Send();
    }

    public void StartRecording()
    {
        IsRecording = true;
        recording = new List<float>();
        currentClip = Microphone.Start(null, true, recordingLength, 44100);
        InvokeRepeating("CopyClipData", recordingLength, recordingLength);
    }

    public AudioClip StopRecording()
    {
        CancelInvoke("CopyClipData");
        int endTime = Microphone.GetPosition(null);
        Microphone.End(null);
        float[] audioSamples = new float[currentClip.samples];
        currentClip.GetData(audioSamples, 0);
        float[] cutSamples = new float[endTime];
        Array.Copy(audioSamples, cutSamples, cutSamples.Length);
        recording.AddRange(cutSamples);
        IsRecording = false;

        return CreateAudioClip();
    }

    private void CopyClipData()
    {
        Debug.Log("Copying clip data");
        float[] audioSamples = new float[currentClip.samples];
        currentClip.GetData(audioSamples, 0);
        recording.AddRange(audioSamples);
        Debug.Log("New record length: " + recording.Count);
    }

    private AudioClip CreateAudioClip()
    {
        AudioClip clip = AudioClip.Create("Recording", recording.Count, 1, 44100, false);
        clip.SetData(recording.ToArray(), 0);
        Debug.Log("Clip length: " + clip.length);
        return clip;
    }
}
