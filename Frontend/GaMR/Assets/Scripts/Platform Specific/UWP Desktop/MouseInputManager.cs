using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseInputManager : MonoBehaviour, IPointingSource, IInputSource
{
    GameObject cursorObject;
    Transform lastObjectHit;
    EventSystem eventSystem;

    public Ray Ray
    {
        get;
        private set;
    }

    public float? ExtentOverride { get { return 10f; } }

    public LayerMask[] PrioritizedLayerMasksOverride { get { return new LayerMask[] { Physics.DefaultRaycastLayers }; } }

    public SupportedInputInfo GetSupportedInputInfo(uint sourceId)
    {
        return SupportedInputInfo.Select;
    }

    public bool OwnsInput(BaseEventData eventData)
    {
        return true;
    }

    public bool SupportsInputInfo(uint sourceId, SupportedInputInfo inputInfo)
    {
        return true;
    }

    public bool TryGetGrasp(uint sourceId, out bool isPressed)
    {
        isPressed = false;
        return false;
    }

    public bool TryGetGripPosition(uint sourceId, out Vector3 position)
    {
        throw new System.NotImplementedException();
    }

    public bool TryGetGripRotation(uint sourceId, out Quaternion rotation)
    {
        throw new System.NotImplementedException();
    }

    public bool TryGetMenu(uint sourceId, out bool isPressed)
    {
        throw new System.NotImplementedException();
    }

    public bool TryGetPointerPosition(uint sourceId, out Vector3 position)
    {
        position = Input.mousePosition;
        return true;
    }

    public bool TryGetPointerRotation(uint sourceId, out Quaternion rotation)
    {
        throw new System.NotImplementedException();
    }

    public bool TryGetPointingRay(uint sourceId, out Ray pointingRay)
    {
        pointingRay = Ray;
        return true;
    }

    public bool TryGetSelect(uint sourceId, out bool isPressed, out double pressedValue)
    {
        isPressed = Input.GetMouseButtonDown(0);
        pressedValue = 1f;
        return true;
    }

    public bool TryGetSourceKind(uint sourceId, out InteractionSourceInfo sourceKind)
    {
        throw new System.NotImplementedException();
    }

    public bool TryGetThumbstick(uint sourceId, out bool isPressed, out Vector2 position)
    {
        throw new System.NotImplementedException();
    }

    public bool TryGetTouchpad(uint sourceId, out bool isPressed, out bool isTouched, out Vector2 position)
    {
        throw new System.NotImplementedException();
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

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Down");
            InputManager.Instance.RaiseSourceDown(this, 0, InteractionSourcePressInfo.Select, null);
        }

        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Clicked");
            InputManager.Instance.RaiseSourceUp(this, 0, InteractionSourcePressInfo.Select, null);
            InputManager.Instance.RaiseInputClicked(this, 0, InteractionSourcePressInfo.Select, 1, null);
        }
    }
}
