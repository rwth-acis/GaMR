using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowTeleport : IconTool
{
    public float startVelocity = 1f;
    public float timeStep = 0.2f;
    public float raycastStep = 0.2f;
    public float timeFactor = 0.1f;
    public float rayThickness = 0.02f;
    public GameObject allowedCursorPrefab;
    public GameObject forbiddenCursorPrefab;
    public Material[] materials;
    public Material[] noTargetMaterials;
    public LayerMask allowedTeleportLayers;

    public Transform uiContainer;

    private const float gravity = 9.81f;

    private GameObject allowedCursorInstance, forbiddenCursorInstance;
    private List<GameObject> curveFollowers;
    private bool teleportAllowed = false;
    private Vector3 targetPosition;
    private GameObject bowParent;
    private float time = 0;
    private const float maxDur = 5f;
    private bool standardMaterialsActive = true;

    private void Start()
    {
        bowParent = new GameObject("Bow Parent");
        bowParent.transform.parent = uiContainer;
        curveFollowers = new List<GameObject>();
        int i = 0;
        for (float t = 0f; t < maxDur; t += timeStep)
        {
            curveFollowers.Add(GameObject.CreatePrimitive(PrimitiveType.Cube));
            i = curveFollowers.Count - 1;
            curveFollowers[i].layer = 1 << LayerMask.NameToLayer("Ignore Raycast") - 1;
            Destroy(curveFollowers[i].GetComponent<BoxCollider>());
            curveFollowers[i].transform.localScale = new Vector3(rayThickness, rayThickness, rayThickness);
            curveFollowers[i].transform.parent = bowParent.transform;
            if (materials.Length > 0)
            {
                int matIndex = i % materials.Length;
                curveFollowers[i].GetComponent<Renderer>().material = materials[matIndex];
            }
            curveFollowers[i].SetActive(false);
        }

        allowedCursorInstance = GameObject.Instantiate(allowedCursorPrefab);
        allowedCursorInstance.transform.parent = bowParent.transform;
        allowedCursorInstance.SetActive(false);
        forbiddenCursorInstance = GameObject.Instantiate(forbiddenCursorPrefab);
        forbiddenCursorInstance.transform.parent = bowParent.transform;
        forbiddenCursorInstance.SetActive(false);
    }

    private void ChangeFollowersMaterial(Material[] newMaterials)
    {
        if (newMaterials.Length > 0)
        {
            for (int i = 0; i < curveFollowers.Count; i++)
            {
                int matIndex = i % newMaterials.Length;
                curveFollowers[i].GetComponent<Renderer>().material = newMaterials[matIndex];
            }
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (bowParent != null)
        {
            bowParent.SetActive(false);
        }
    }

    private void Update()
    {
        if (Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {
            bowParent.SetActive(true);
            BowUpdate();
        }
        else
        {
            bowParent.SetActive(false);
        }

        if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            GridManager.Instance.ShowGrids();
        }

        if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            if (teleportAllowed)
            {
                Vector3 offset = Camera.main.transform.localPosition;
                GameObject.FindGameObjectWithTag("Player").transform.position = targetPosition - new Vector3(offset.x, 0, offset.z);
            }

            GridManager.Instance.HideGrids();
        }
    }

    private void BowUpdate()
    {
        if (Mathf.Abs(timeStep) < 0.01f)
        {
            timeStep = 0.01f;
        }
        if (Mathf.Abs(raycastStep) < 0.01f)
        {
            raycastStep = 0.01f;
        }
        RaycastHit hit;
        float timePoint;
        float timeLimit = maxDur;

        Vector3 startPos = transform.position - transform.forward * 0.16f - transform.up * 0.015f;
        float verticalAngle = -transform.eulerAngles.x;
        float horizontalAngle = transform.eulerAngles.y - 90f;

        if (BowRaycast(startPos, verticalAngle, horizontalAngle, out hit, out timePoint))
        {
            timeLimit = timePoint;
            targetPosition = hit.point;

            int hitLayer = hit.collider.gameObject.layer;
            if (((1 << hitLayer) & allowedTeleportLayers) == 0 || Vector3.Dot(hit.normal, Vector3.up) <= 0)
            {
                allowedCursorInstance.SetActive(false);
                forbiddenCursorInstance.SetActive(true);
                forbiddenCursorInstance.transform.position = hit.point + hit.normal * 0.02f; // place slightly above surface
                forbiddenCursorInstance.transform.rotation = Quaternion.LookRotation(hit.normal);
                teleportAllowed = false;

                if (standardMaterialsActive)
                {
                    standardMaterialsActive = false;
                    ChangeFollowersMaterial(noTargetMaterials);
                }
            }
            else
            {
                forbiddenCursorInstance.SetActive(false);
                allowedCursorInstance.SetActive(true);
                allowedCursorInstance.transform.position = hit.point + hit.normal * 0.02f; // place slightly above surface
                allowedCursorInstance.transform.rotation = Quaternion.LookRotation(hit.normal);
                teleportAllowed = true;

                if (!standardMaterialsActive)
                {
                    standardMaterialsActive = true;
                    ChangeFollowersMaterial(materials);
                }
            }
        }
        else
        {
            if (standardMaterialsActive)
            {
                standardMaterialsActive = false;
                ChangeFollowersMaterial(noTargetMaterials);
            }
            allowedCursorInstance.SetActive(false);
            forbiddenCursorInstance.SetActive(false);
            teleportAllowed = false;
        }

        time += Time.deltaTime * timeFactor;

        DrawBow(startPos, verticalAngle, horizontalAngle, time, timeLimit);

        if (time > timeLimit)
        {
            time = 0;
        }
    }

    private bool BowRaycast(Vector3 startPoint, float angle, float horizontalAngle, out RaycastHit hit, out float timePoint)
    {
        for (float t = 0; t < maxDur; t += raycastStep)
        {
            RaycastHit lineHit;
            if (Physics.Linecast(BowPoint(t, startPoint, angle, horizontalAngle), BowPoint(t + raycastStep, startPoint, angle, horizontalAngle), out lineHit))
            {
                hit = lineHit;
                timePoint = t + raycastStep;
                return true;
            }
        }

        timePoint = -1;
        hit = default(RaycastHit);
        return false;
    }

    private void DrawBow(Vector3 startPoint, float angle, float horizontalAngle, float timeOfFirst, float timeLimit)
    {
        for (int i = 0; i < curveFollowers.Count; i++)
        {
            float time = timeOfFirst + timeStep * i;
            if (time / timeLimit >= 2f)
            {
                curveFollowers[i].SetActive(false);
            }
            else
            {
                curveFollowers[i].SetActive(true);
                time %= timeLimit;
                Vector3 pos = BowPoint(time, startPoint, angle, horizontalAngle);
                Vector3 nextPos = BowPoint(time + timeStep, startPoint, angle, horizontalAngle);
                Vector3 diff = nextPos - pos;
                curveFollowers[i].transform.position = Vector3.Lerp(pos, nextPos, 0.5f);
                Quaternion rotation = Quaternion.LookRotation(diff);
                curveFollowers[i].transform.rotation = rotation;
                curveFollowers[i].transform.localScale = new Vector3(
                    curveFollowers[i].transform.localScale.x,
                    curveFollowers[i].transform.localScale.y,
                    diff.magnitude
                    );
            }
        }
    }

    private Vector3 BowPoint(float time, Vector3 startPoint, float angle, float horizontalAngle)
    {
        Quaternion rotation = Quaternion.Euler(0, horizontalAngle, 0);
        Vector3 point = new Vector3(
            startVelocity * time * Mathf.Cos(angle * Mathf.Deg2Rad),
            startVelocity * time * Mathf.Sin(angle * Mathf.Deg2Rad) - 1 / 2 - gravity * Mathf.Pow(time, 2),
            0
            );

        point = rotation * point;

        return startPoint + point;
    }
}
