using HoloToolkit.Unity.InputModule;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script which replaces tap to place with a custom solution, e.g. on the menus
/// Used only on non-XR versions of the framework
/// replacement is required since the Holotoolkit's input module is deactivated
/// </summary>
public class ReplaceTapToPlaceScripts : MonoBehaviour
{

    public float checkInterval = 5f;

    private void Awake()
    {
        if (UnityEngine.XR.XRSettings.enabled)
        {
            Destroy(this);
        }
    }

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    private void OnSceneChanged(Scene scene1, Scene scene2)
    {
        StartCoroutine(ReplaceTapToPlace());
    }

    private IEnumerator ReplaceTapToPlace()
    {
        CustomTapToPlace[] tapToPlaceScripts = UnityEngine.Object.FindObjectsOfType<CustomTapToPlace>();

        Debug.Log("Replacing Tap to Place... Found " + tapToPlaceScripts.Length + " to replace");

        for (int i = 0; i < tapToPlaceScripts.Length; i++)
        {
            GameObject attachedTo = tapToPlaceScripts[i].gameObject;
            tapToPlaceScripts[i].IsBeingPlaced = false;
            yield return null;
            Destroy(tapToPlaceScripts[i]);
            attachedTo.gameObject.AddComponent<KeepInFrontOfCamera>();
        }
    }
}
