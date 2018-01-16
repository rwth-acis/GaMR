using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioProgressBar : MonoBehaviour
{
    private Transform innerProgressBar;
    private TextMesh timeLabel;

    [SerializeField]
    private AudioSource source;

    public AudioSource Source
    {
        get
        { return source; }
        set
        {
            source = value;
        }
    }

    // Use this for initialization
    void Start()
    {
        innerProgressBar = transform.Find("Audio Progress Bar/Audio Progress Bar Inner Part");
        timeLabel = transform.Find("Time Label").GetComponent<TextMesh>();
    }

    private void Update()
    {
        if (Source != null && Source.isPlaying)
        {

            timeLabel.text = SecondsToTimeString(Source.time) + "/" + SecondsToTimeString(Source.clip.length);
            innerProgressBar.localScale = new Vector3(
                Source.time / source.clip.length,
                1,
                1
                );
        }
    }

    private static string SecondsToTimeString(float seconds)
    {
        int minutes = (int) (seconds / 60);
        int secondsOfMinute = Mathf.RoundToInt(seconds % 60);
        return minutes.ToString("00") + ":" + secondsOfMinute.ToString("00");
    }
}
