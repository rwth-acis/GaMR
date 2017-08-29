using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BadgeManager : MonoBehaviour, IInputClickHandler
{

    private Badge badge;
    private Material mat;
    private ConstantRotation rotationScript;
    private BoundingBoxActions boundingBoxActions;

    public Badge Badge
    {
        get { return badge; }
        set
        {
            badge = value;
            if (badge != null)
            {
                //Mat.SetTexture(badge.Name, badge.Image);
                Mat.mainTexture = badge.Image;
            }
        }
    }

    private Material Mat
    {
        get
        {
            if (mat == null)
            {
                mat = GetComponent<Renderer>().materials[1];
            }
            return mat;
        }
    }

    private void Start()
    {
        rotationScript = GetComponent<ConstantRotation>();
    }

    public void WinBadge()
    {
        // un-parent from the progress bar
        transform.parent = null;
        StartCoroutine(MoveInFrontOfCamera(3f, 2f));
    }

    IEnumerator MoveInFrontOfCamera(float movementTime, float rotationHighlightTime)
    {
        float elapsedTime = 0f;
        Vector3 start = transform.position;

        while(elapsedTime < movementTime)
        {
            transform.position = Vector3.Lerp(start, InFrontOfCamera.Instance.Position, (elapsedTime / movementTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.parent = InFrontOfCamera.Instance.transform;
        transform.position = InFrontOfCamera.Instance.Position;


        float initialRotationSpeed = rotationScript.degreesPerSecond;

        yield return HighlightRotation(rotationHighlightTime/3f, 2000f);
        yield return HighlightRotation(rotationHighlightTime, initialRotationSpeed);

        yield return new WaitForSeconds(2f);

        Destroy(gameObject);
    }

    IEnumerator HighlightRotation(float highlightTime, float targetRotationsPerSecond)
    {
        float initialRotationSpeed = rotationScript.degreesPerSecond;
        float elapsedTime = 0f;

        while(elapsedTime < highlightTime)
        {
            rotationScript.degreesPerSecond = Mathf.Lerp(initialRotationSpeed, targetRotationsPerSecond, (elapsedTime / highlightTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rotationScript.degreesPerSecond = targetRotationsPerSecond;
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        if (InformationManager.Instance.playerType != PlayerType.STUDENT)
        {
            BadgeEditor.ShowBadges();
        }
    }
}
