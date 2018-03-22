using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseInputManager : MonoBehaviour, IPointingSource
{
    GameObject cursorObject;
    Transform lastObjectHit;
    InteractionInputSource inputSource;
    EventSystem eventSystem;

    public Ray Ray
    {
        get;
        private set;
    }

    public float? ExtentOverride { get { return 10f; } }

    public LayerMask[] PrioritizedLayerMasksOverride { get { return new LayerMask[] { Physics.DefaultRaycastLayers }; } }

    public bool OwnsInput(BaseEventData eventData)
    {
        return true;
    }

    public void UpdatePointer()
    {
        Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    }

    private void Awake()
    {
        cursorObject = GameObject.FindGameObjectWithTag("Cursor");
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        FocusManager.Instance.RegisterPointer(this);
    }

    private void FixedUpdate()
    {
        Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Debug.DrawRay(Ray.origin, Ray.direction);

        if (Physics.Raycast(Ray, out hit, 100f))
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
                InputManager.Instance.RaiseInputClicked(null, 0, InteractionSourcePressInfo.Select, 1, null);
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
            cursorObject.transform.position = Ray.origin + 2 * Ray.direction;
        }
    }
}
