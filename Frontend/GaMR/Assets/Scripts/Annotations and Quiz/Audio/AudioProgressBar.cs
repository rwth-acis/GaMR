using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioProgressBar : MonoBehaviour
{
    private Transform innerProgressBar; // reference to the innter progress bar
    private TextMesh timeLabel; // reference to the time display

    /// <summary>
    /// The audio source which is monitored by the progress bar. Its progress is visualized.
    /// </summary>
    public AudioSource Source
    {
        get;set;
    }

    /// <summary>
    /// true if a recording is currently in progress
    /// </summary>
    public bool DisplayRecording
    {
        get;set;
    }

    /// <summary>
    /// Gets the necessary components and references
    /// </summary>
    void Start()
    {
        innerProgressBar = transform.Find("Audio Progress Bar/Audio Progress Bar Inner Part");
        timeLabel = transform.Find("Time Label").GetComponent<TextMesh>();
    }

    /// <summary>
    /// Decides what the progress bar should display:
    /// If a recording is in progress, it shows the length of the recording
    /// If a clip was saved, it shows how long this clip is and how much of it was already played
    /// </summary>
    private void Update()
    {
        if (DisplayRecording)
        {
            if (timeLabel == null || innerProgressBar == null)
            {
                Start();
            }
            timeLabel.text = "Recording: " + SecondsToTimeString(RecordingManager.Instance.CurrentRecordingLength);
            // display the current amplitude on the progress bar
            float peakAmplitude = RecordingManager.Instance.PeakAmplitude;
            peakAmplitude = Math.Min(1, peakAmplitude * 10);
            innerProgressBar.localScale = Vector3.Lerp(innerProgressBar.localScale, new Vector3(peakAmplitude, 1, 1), Time.deltaTime * 3f);
        }
        else
        {
            if (Source != null && Source.isPlaying)
            {
                UpdateProgressBar();
            }
        }
    }

    /// <summary>
    /// Updates the progress bar to a clip
    /// Determines how long a clip is and how much of it has already been played
    /// Adapts the label and the progress bar length accordingly
    /// </summary>
    public void UpdateProgressBar()
    {
        if (timeLabel == null || innerProgressBar == null)
        {
            Start();
        }
        if (Source != null && Source.clip != null)
        {
            timeLabel.text = SecondsToTimeString(Source.time) + "/" + SecondsToTimeString(Source.clip.length);
            innerProgressBar.localScale = new Vector3(
                Source.time / Source.clip.length,
                1,
                1
                );
        }
        else
        {
            timeLabel.text = "0:00 / 0:00";
        }
    }

    /// <summary>
    /// Converts a float which represents an amount of seconds to a time string of the form mm:ss
    /// </summary>
    /// <param name="seconds">The duration in seconds</param>
    /// <returns>A string of the form "mm:ss" which represents a time of minutes and seconds</returns>
    private static string SecondsToTimeString(float seconds)
    {
        int minutes = (int) (seconds / 60);
        int secondsOfMinute = Mathf.RoundToInt(seconds % 60);
        return minutes.ToString("00") + ":" + secondsOfMinute.ToString("00");
    }
}
