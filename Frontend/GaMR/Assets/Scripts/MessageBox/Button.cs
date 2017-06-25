using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Button : MonoBehaviour, IInputHandler {

    public Action OnPressed;

    public void OnInputDown(InputEventData eventData)
    {
        if (OnPressed!=null)
        {
            OnPressed();
        }
    }

    public void OnInputUp(InputEventData eventData)
    {
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
