using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HoloToolkit.Sharing.Tests;
using HoloToolkit.Sharing;

/// <summary>
/// Transforms the attached gameobject
/// Provides a collection of shortcut-methods for transformations
/// </summary>
public class TransformationManager : MonoBehaviour
{
    [Tooltip("The maximum size of the gameObject's bounds")]
    public Vector3 maxSize;
    [Tooltip("The minimum size of the gameObject's bounds")]
    public Vector3 minSize;
    private Rigidbody ridgidBody;
    private BoundingBoxInfo boxInfo;
    private static Dictionary<int, TransformationManager> instances;

    private void Start()
    {
        ridgidBody = GetComponent<Rigidbody>();
        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.BoundingBoxTransform] = ReceivedRemoteTransformChange;
        boxInfo = GetComponentInChildren<BoundingBoxInfo>();
        if (boxInfo == null)
        {
            Debug.LogError("Bounding Box needs to have a BoundingBoxInfo");
        }
        if (instances == null)
        {
            instances = new Dictionary<int, TransformationManager>();
        }
        instances.Add(boxInfo.BoxId, this);
    }

    private void OnDestroy()
    {
        instances.Remove(boxInfo.BoxId);
    }

    private static void ReceivedRemoteTransformChange(NetworkInMessage msg)
    {
        msg.ReadInt64(); // this is the user ID
        int msgBoundingBoxId = msg.ReadInt32();
        Vector3 newPosition = CustomMessages.Instance.ReadVector3(msg);
        Quaternion newRotation = CustomMessages.Instance.ReadQuaternion(msg);
        Vector3 newScale = CustomMessages.Instance.ReadVector3(msg);

        if (instances.ContainsKey(msgBoundingBoxId))
        {
            instances[msgBoundingBoxId].OnRemoteTransformChanged(newPosition, newRotation, newScale);
        }
        else
        {
            Debug.LogError("Received transform from an unknown bounding box");
        }
    }

    private void OnRemoteTransformChanged(Vector3 newPosition, Quaternion newRotation, Vector3 newScale)
    {
        transform.localPosition = newPosition;
        transform.localRotation = newRotation;
        transform.localScale = newScale;
    }

    private void UpdateTransformToRemote()
    {
        CustomMessages.Instance.SendBoundingBoxTransform(boxInfo.BoxId, transform.localPosition, transform.localRotation, transform.localScale);
    }

    public void Scale(Vector3 scaleVector)
    {
        Vector3 newScale = new Vector3(
            transform.localScale.x * scaleVector.x,
            transform.localScale.y * scaleVector.y,
            transform.localScale.z * scaleVector.z);
        // if there are size restrictions defined => first check if they are violated
        // check restrictions
        if ((newScale.x <= maxSize.x && newScale.y <= maxSize.y && newScale.z <= maxSize.z) &&
            (newScale.x >= minSize.x && newScale.y >= minSize.y && newScale.z >= minSize.z))
        {
            transform.localScale = newScale;

        }

        UpdateTransformToRemote();
    }

    public void Rotate(Vector3 axis, float angle)
    {
        transform.Rotate(axis, angle, Space.World);
        UpdateTransformToRemote();
    }

    internal void Translate(Vector3 movement)
    {
        RaycastHit hitInfo;
        // check if the object would collide with something
        // if not => perform the movement
        bool collidesWithSomething = ridgidBody.SweepTest(movement, out hitInfo, movement.magnitude);
        if (!collidesWithSomething)
        {
            transform.Translate(movement, Space.World);
        }

        UpdateTransformToRemote();
    }

    private IEnumerator SmoothRotate(Vector3 axis, float angle, float time)
    {
        Quaternion startingRotation = transform.localRotation;
        Rotate(axis, angle);
        Quaternion targetRotation = transform.localRotation;
        transform.localRotation = startingRotation;

        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(startingRotation, targetRotation, (elapsedTime / time));

            yield return new WaitForEndOfFrame();
        }
        transform.localRotation = targetRotation;
        UpdateTransformToRemote();
    }

    private IEnumerator SmoothScale(Vector3 scaleVector, float time)
    {
        Vector3 startingScale = transform.localScale;
        Scale(scaleVector);
        Vector3 targetScale = transform.localScale;
        transform.localScale = startingScale;

        float elapsedTime = 0;
        while(elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            transform.localScale = Vector3.Slerp(startingScale, targetScale, (elapsedTime / time));

            yield return new WaitForEndOfFrame();
        }

        transform.localScale = targetScale;
        UpdateTransformToRemote();
    }

    // ---------------------------------
    // quick commands for voice-control:

    public void ScaleUp()
    {
        StartCoroutine(SmoothScale(new Vector3(1.1f, 1.1f, 1.1f), 1f));
    }

    public void ScaleDown()
    {
        StartCoroutine(SmoothScale(new Vector3(0.9f, 0.9f,0.9f), 1f));
    }

    public void RotateCW()
    {
        StartCoroutine(SmoothRotate(Vector3.up, 45, 1f));
    }

    public void RotateCCW()
    {
        StartCoroutine(SmoothRotate(Vector3.up, -45, 1f));
    }
}
