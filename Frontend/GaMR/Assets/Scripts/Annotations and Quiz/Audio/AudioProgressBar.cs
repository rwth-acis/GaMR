using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioProgressBar : MonoBehaviour
{
    private Transform innerProgressBar;
    private TextMesh timeLabel;

    /// <summary>
    /// The audio source which is monitored by the progress bar. Its progress is visualized.
    /// </summary>
    public AudioSource Source
    {
        get;set;
    }

    public bool DisplayRecording
    {
        get;set;
    }

    // Use this for initialization
    void Start()
    {
        innerProgressBar = transform.Find("Audio Progress Bar/Audio Progress Bar Inner Part");
        timeLabel = transform.Find("Time Label").GetComponent<TextMesh>();
    }

    private void Update()
    {
        if (DisplayRecording)
        {
            if (timeLabel == null || innerProgressBar == null)
            {
                Start();
            }
            timeLabel.text = "Recording: " + SecondsToTimeString(RecordingManager.Instance.CurrentRecordingLength);
        }
        else
        {
            if (Source != null && Source.isPlaying)
            {
                UpdateProgressBar();
            }
        }
    }

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

    private static string SecondsToTimeString(float seconds)
    {
        int minutes = (int) (seconds / 60);
        int secondsOfMinute = Mathf.RoundToInt(seconds % 60);
        return minutes.ToString("00") + ":" + secondsOfMinute.ToString("00");
    }
}
