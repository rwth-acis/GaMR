using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnnotationMenu : BaseMenu
{
    private FocusableButton editButton, deleteButton, closeButton, playPauseButton, recordAudioButton, stopButton;
    private TextMesh label;
    private Caption caption;
    private AnnotationContainer container;
    private AudioState audioState;

    private AudioState AudioState
    {
        get
        {
            return audioState;
        }
        set
        {
            // adapt the UI according to the new audio state
            UpdateAudioState(audioState, value);
            UpdateButtonsToAudioState(audioState, value);
            audioState = value;
        }
    }

    private Coroutine flashRoutine;

    private static AnnotationMenu currentlyOpenAnnotationMenu;

    [SerializeField]
    private Sprite playIcon;
    [SerializeField]
    private Sprite pauseIcon;

    public AnnotationContainer Container
    {
        get
        {
            return container;
        }
        set
        {
            container = value;
            if (editButton != null)
            {
                editButton.ButtonEnabled = (container != null);
            }
            if (deleteButton != null)
            {
                deleteButton.ButtonEnabled = (container != null);
            }
        }
    }

    protected override void Start()
    {
        base.Start();
        InitializeButtons();
        label = transform.Find("Label").GetComponent<TextMesh>();
        caption = transform.Find("TextField").GetComponent<Caption>();
        caption.Init();

        if (Container != null)
        {
            caption.Text = Container.Annotation.Text;
            if (container.AnnotationClip == null)
            {
                AudioState = AudioState.NONE_RECORDED;
            }
            else
            {
                AudioState = AudioState.STOPPED;
            }
        }
        else
        {
            caption.Text = "No annotation associated!";
        }

        if (currentlyOpenAnnotationMenu != null)
        {
            currentlyOpenAnnotationMenu.Close();
        }
        currentlyOpenAnnotationMenu = this;
    }

    private void InitializeButtons()
    {
        closeButton = transform.Find("Close Button").GetComponent<FocusableButton>();
        deleteButton = transform.Find("Delete Button").GetComponent<FocusableButton>();
        editButton = transform.Find("Edit Button").GetComponent<FocusableButton>();
        playPauseButton = transform.Find("Play Button").GetComponent<FocusableButton>();
        recordAudioButton = transform.Find("Record Button").GetComponent<FocusableButton>();
        stopButton = transform.Find("Stop Button").GetComponent<FocusableButton>();


        closeButton.OnPressed = Close;
        deleteButton.OnPressed = DeleteAnnotation;
        editButton.OnPressed = EditText;
        playPauseButton.OnPressed = PlayPauseAudio;
        recordAudioButton.OnPressed = RecordAudio;
        stopButton.OnPressed = StopAudio;

        // if the window somehow got instantiated without an attached container: disable the edit and delete button
        if (Container == null)
        {
            editButton.ButtonEnabled = false;
            deleteButton.ButtonEnabled = false;
        }

        // disable the record button if there is no microphone
        if (recordAudioButton != null && Microphone.devices.Length == 0)
        {
            recordAudioButton.ButtonEnabled = false;
        }

    }

    private void Close()
    {
        currentlyOpenAnnotationMenu = null;
        if (Container != null)
        {
            Container.Deselect();
        }
        Destroy(gameObject);
    }

    private void DeleteAnnotation()
    {
        if (Container != null)
        {
            Container.DeleteAnnotation();
        }
        Close();
    }

    /// <summary>
    /// Called if the edit-button is pressed => opens a keyboard to edit the annotation
    /// </summary>
    private void EditText()
    {
        Keyboard.Display("Edit the annotation", Container.Annotation.Text, OnEditFinished, true);
        gameObject.SetActive(false);

    }

    /// <summary>
    /// called if the edit-keyboard is closed
    /// applies changes to the annotation-text
    /// </summary>
    /// <param name="input">The text which was typed by the user (null if input was cancelled)</param>
    private void OnEditFinished(string input)
    {
        if (input != null)
        {
            Container.EditAnnotation(input);
            caption.Text = input;
        }
        gameObject.SetActive(true);

    }

    private void PlayPauseAudio()
    {
        if (AudioState == AudioState.PLAYING)
        {
            AudioState = AudioState.PAUSED;
        }
        else
        {
            AudioState = AudioState.PLAYING;
        }
    }

    private void StopAudio()
    {
        AudioState = AudioState.STOPPED;
    }

    private void UpdateButtonsToAudioState(AudioState previousState, AudioState newState)
    {

        Debug.Log("Changing from " + previousState + " to " + newState);

        if (previousState == AudioState.RECORDING)
        {
            StopCoroutine(flashRoutine);
            recordAudioButton.IconVisible = true; // make sure that the icon is displayed

            // re-enable the play and stop buttons
            playPauseButton.ButtonEnabled = true;
            stopButton.ButtonEnabled = true;
        }

        switch (newState)
        {
            case AudioState.NONE_RECORDED:
                playPauseButton.ButtonEnabled = false;
                stopButton.ButtonEnabled = false;
                break;
            case AudioState.PLAYING:
                playPauseButton.Text = LocalizationManager.Instance.ResolveString("Pause");
                playPauseButton.Icon = pauseIcon;

                stopButton.ButtonEnabled = true;
                break;
            case AudioState.PAUSED:
                // if previously playing => now not playing and so reset the play-button icon
                playPauseButton.Text = LocalizationManager.Instance.ResolveString("Play");
                playPauseButton.Icon = playIcon;

                stopButton.ButtonEnabled = true;

                break;
            case AudioState.STOPPED:
                // if previously playing => now not playing and so reset the play-button icon
                playPauseButton.Text = LocalizationManager.Instance.ResolveString("Play");
                playPauseButton.Icon = playIcon;

                stopButton.ButtonEnabled = false;

                break;
            case AudioState.RECORDING:
                flashRoutine = StartCoroutine(FlashRecordIcon());
                // also disable the play and stop button
                playPauseButton.ButtonEnabled = false;
                stopButton.ButtonEnabled = false;

                break;
        }
    }

    private void UpdateAudioState(AudioState previousState, AudioState newState)
    {
        // if a recording was active => stop it and store the annotation
        if (previousState == AudioState.RECORDING && RecordingManager.Instance.IsRecording)
        {
            AudioClip recordedClip = RecordingManager.Instance.StopRecording();
            if (recordedClip != null)
            {
                container.AnnotationClip = recordedClip;
            }
        }

        switch (newState)
        {
            case AudioState.NONE_RECORDED:
                break;
            case AudioState.PLAYING:
                if (previousState == AudioState.PAUSED)
                {
                    container.AnnotationAudioSource.UnPause();
                }
                else
                {
                    container.AnnotationAudioSource.Play();
                }
                break;
            case AudioState.PAUSED:
                container.AnnotationAudioSource.Pause();
                break;
            case AudioState.STOPPED:
                container.AnnotationAudioSource.Stop();
                break;
            case AudioState.RECORDING:
                // start the recording; if there is an error => stop again
                if (!RecordingManager.Instance.StartRecording())
                {
                    if (previousState == AudioState.NONE_RECORDED)
                    {
                        AudioState = AudioState.NONE_RECORDED;
                    }
                    else
                    {
                        AudioState = AudioState.STOPPED;
                    }
                    MessageBox.Show("No microphone found", MessageBoxType.ERROR);
                }
                break;
        }
    }

    private void RecordAudio()
    {
        // if a recording is active => stop it
        if (AudioState == AudioState.RECORDING)
        {
            Debug.Log("Stop Recording");
            AudioState = AudioState.STOPPED;
        }
        else // no recording active => start a new one
        {
            Debug.Log("Start Recording");
            AudioState = AudioState.RECORDING;
        }
    }

    private IEnumerator FlashRecordIcon()
    {
        float flashPause = 0.5f;
        //  the loop needs to be terminated by stopping the coroutine
        while (true)
        {
            recordAudioButton.IconVisible = false;
            yield return new WaitForSeconds(flashPause);
            recordAudioButton.IconVisible = true;
            yield return new WaitForSeconds(flashPause);
        }
    }

    private void Update()
    {
        if (container != null)
        {
            // if the audio should be playing => check if it is stoped by something else (e.g. the clip ends)
            if (AudioState == AudioState.PLAYING)
            {
                if (!container.AnnotationAudioSource.isPlaying)
                {
                    AudioState = AudioState.STOPPED;
                }
            }
        }
    }

    public override void OnUpdateLanguage()
    {
        base.OnUpdateLanguage();
        closeButton.Text = LocalizationManager.Instance.ResolveString("Close");
        deleteButton.Text = LocalizationManager.Instance.ResolveString("Delete");
        editButton.Text = LocalizationManager.Instance.ResolveString("Edit");

        recordAudioButton.Text = LocalizationManager.Instance.ResolveString("Record");
        playPauseButton.Text = LocalizationManager.Instance.ResolveString("Play");
        stopButton.Text = LocalizationManager.Instance.ResolveString("Stop");

        label.text = LocalizationManager.Instance.ResolveString("Annotation");
    }
}

public enum AudioState
{
    NONE_RECORDED, PLAYING, STOPPED, PAUSED, RECORDING
}
