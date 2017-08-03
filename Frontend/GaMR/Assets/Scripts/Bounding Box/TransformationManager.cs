using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    private void Start()
    {
        ridgidBody = GetComponent<Rigidbody>();
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
    }

    public void Rotate(Vector3 axis, float angle)
    {
        transform.Rotate(axis, angle, Space.World);
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
