using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIMessage : Singleton<UIMessage> {

    Text messageText;

    public bool Active
    {
        get { return gameObject.activeSelf; }
    }

	// Use this for initialization
	private void Start () {

        messageText = GetComponent<Text>();
        gameObject.SetActive(false);
	}

    public void Show(string text)
    {
        gameObject.SetActive(true);
        messageText.text = text;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void ShowFor(string text, float seconds)
    {
        StartCoroutine(ShowForCoroutine(text, seconds));
    }

    private IEnumerator ShowForCoroutine(string text, float seconds)
    {
        gameObject.SetActive(true);
        yield return new WaitForSeconds(seconds);
        gameObject.SetActive(false);
    }
}
