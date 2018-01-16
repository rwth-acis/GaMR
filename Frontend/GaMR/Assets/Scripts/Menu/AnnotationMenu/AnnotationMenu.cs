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
    private bool audioPlaying;

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

        // if the window somehow got instantiated without an attached container: disable the edit and delete button
        if (Container == null)
        {
            editButton.ButtonEnabled = false;
            deleteButton.ButtonEnabled = false;
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
        audioPlaying = !audioPlaying;
        if (audioPlaying)
        {
            playPauseButton.Text = LocalizationManager.Instance.ResolveString("Pause");
            playPauseButton.Icon = pauseIcon;
        }
        else
        {
            playPauseButton.Text = LocalizationManager.Instance.ResolveString("Play");
            playPauseButton.Icon = playIcon;
        }
    }

    private void RecordAudio()
    {
        // if a recording is active => stop it
        if (RecordingManager.Instance.IsRecording)
        {
            Debug.Log("Stop Recording");

            RecordingManager.Instance.StopRecording();

            StopCoroutine(flashRoutine);
            recordAudioButton.IconVisible = true; // make sure that the icon is displayed
            // re-enable the play and stop buttons
            playPauseButton.ButtonEnabled = true;
            stopButton.ButtonEnabled = true;
        }
        else // no recording active => start a new one
        {
            Debug.Log("Start Recording");
            flashRoutine = StartCoroutine(FlashRecordIcon());
            // also disable the play and stop button
            playPauseButton.ButtonEnabled = false;
            stopButton.ButtonEnabled = false;
            RecordingManager.Instance.StartRecording();
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
        //// if the playing state changes => change the icon on the play button
        //if (audioPlaying != container.AudioAnnotationSource.isPlaying)
        //{
        //    if (container.AudioAnnotationSource.isPlaying)
        //    {

        //    }
        //    else
        //    {

        //    }
        //}
        //audioPlaying = container.AudioAnnotationSource.isPlaying;
    }

    public override void OnUpdateLanguage()
    {
        base.OnUpdateLanguage();
        closeButton.Text = LocalizationManager.Instance.ResolveString("Close");
        deleteButton.Text = LocalizationManager.Instance.ResolveString("Delete");
        editButton.Text = LocalizationManager.Instance.ResolveString("Edit");

        label.text = LocalizationManager.Instance.ResolveString("Annotation");
    }
}
