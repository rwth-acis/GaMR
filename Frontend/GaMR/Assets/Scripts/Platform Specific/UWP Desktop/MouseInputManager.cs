﻿using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseInputManager : MonoBehaviour
{
    GameObject cursorObject;
    Transform lastObjectHit;
    InteractionInputSource inputSource;

    private void Awake()
    {
        cursorObject = GameObject.FindGameObjectWithTag("Cursor");
    }

    private void FixedUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10f))
        {
            cursorObject.transform.position = hit.point - 0.02f * (hit.point - Camera.main.transform.position);
            Quaternion.LookRotation(hit.normal);

            Transform objectHit = hit.transform;

            if (Input.GetMouseButton(0))
            {
                Debug.Log("Clicked: " + objectHit);
            }

            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Down: " + objectHit);
            }

            if (objectHit != lastObjectHit)
            {
                if (objectHit != null)
                {
                    InputManager.Instance.RaiseFocusEnter(objectHit.gameObject);
                }
                if (lastObjectHit != null)
                {
                    InputManager.Instance.RaiseFocusExit(lastObjectHit.gameObject);
                }

                lastObjectHit = objectHit;
            }
        }
        else
        {
            cursorObject.transform.position = ray.origin + 2 * ray.direction;
        }
    }
}
