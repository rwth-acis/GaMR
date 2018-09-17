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
    private const int recordingLength = 60;

    /// <summary>
    /// true if a recording is currently active
    /// </summary>
    public bool IsRecording
    {
        get; private set;
    }

    /// <summary>
    /// the length of the current recording in seconds. 0 if on recording is active
    /// </summary>
    public float CurrentRecordingLength
    {
        get;
        private set;
    }

    /// <summary>
    /// Test-routine for debugging
    /// </summary>
    /// <returns></returns>
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
        UnityWebRequest post = new UnityWebRequest("http://localhost:8080/resources/annotation/audio/brain/test", "POST");
        post.uploadHandler = new UploadHandlerRaw(oggBytes);
        yield return post.Send();
    }

    /// <summary>
    /// Starts a recording
    /// </summary>
    /// <returns>true if the recording was successfully started</returns>
    public bool StartRecording()
    {
        if (Microphone.devices.Length > 0)
        {
            CurrentRecordingLength = 0;
            IsRecording = true;
            recording = new List<float>();
            currentClip = Microphone.Start(null, true, recordingLength, 44100);
            InvokeRepeating("CopyClipData", recordingLength, recordingLength);
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Stops the recording currently active recording
    /// </summary>
    /// <returns>the full audio clip of the recording</returns>
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
        CurrentRecordingLength = 0;

        return CreateAudioClip();
    }

    /// <summary>
    /// Copies the samples of a limited audio clip to the overall recording samples
    /// This is required since it is only possible to start recordings with a predefined length
    /// For recordings with an arbitrary length, many of these fixed-length clips are concatenated
    /// </summary>
    private void CopyClipData()
    {
        Debug.Log("Copying clip data");
        float[] audioSamples = new float[currentClip.samples];
        currentClip.GetData(audioSamples, 0);
        recording.AddRange(audioSamples);
        Debug.Log("New record length: " + recording.Count);
    }

    /// <summary>
    /// Converts the current list of recording samples to an audio clip
    /// </summary>
    /// <returns>The audio clip created from the recording</returns>
    private AudioClip CreateAudioClip()
    {
        AudioClip clip = AudioClip.Create("Recording", recording.Count, 1, 44100, false);
        clip.SetData(recording.ToArray(), 0);
        Debug.Log("Clip length: " + clip.length);
        return clip;
    }

    /// <summary>
    /// Updates the recording length so that it can be displayed
    /// </summary>
    private void Update()
    {
        if (IsRecording)
        {
            CurrentRecordingLength += Time.deltaTime;
        }
    }

    /// <summary>
    /// returns the peak amplitude of the microphone in the last 100 samples
    /// </summary>
    public float PeakAmplitude
    {
        get
        {
            int samplesBack = 100;
            if (currentClip != null)
            {
                float[] data = new float[samplesBack];
                int microphoneStartPosition = Microphone.GetPosition(null) - (samplesBack + 1);
                if (microphoneStartPosition < 0)
                {
                    return 0;
                }
                currentClip.GetData(data, microphoneStartPosition);

                float maxAmplitude = 0;
                for (int i = 0; i < samplesBack; i++)
                {
                    if (maxAmplitude < data[i])
                    {
                        maxAmplitude = data[i];
                    }
                }

                return maxAmplitude;
            }
            else
            {
                return 0;
            }
        }
    }
}
