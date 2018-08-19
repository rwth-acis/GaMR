using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Laser pointer which draws a ray from the controller to the next point
/// </summary>
public class LaserPointer : Tool
{

    public bool showAlways = true;
    public LayerMask mask = -1;
    public Color color;
    public Material inactiveMaterial;
    public GameObject laserPrefab;
    public GameObject hitPrefab;

    public Vector3 cursorSize = new Vector3(0.015f, 0.015f, 0.015f);
    public Vector3 cursorPressedSize = new Vector3(0.008f, 0.008f, 0.008f);

    private Material laserMaterial;

    private Renderer laserRenderer;
    private GameObject laser;
    private Vector3 hitPoint;
    private GameObject hitInstance;

    private void Start()
    {
        laser = Instantiate(laserPrefab);
        laserRenderer = laser.GetComponent<Renderer>();
        laserMaterial = laserRenderer.material;
        hitInstance = Instantiate(hitPrefab);
    }

    private void ShowLaser(RaycastHit hit)
    {
        laser.SetActive(true);
        laserRenderer.material = laserMaterial;
        laserMaterial.color = color;
        laser.transform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
        laser.transform.LookAt(hitPoint);
        laser.transform.localScale = new Vector3(laser.transform.localScale.x, laser.transform.localScale.y,
            hit.distance);
    }

    private void ShowDefaultLaser()
    {
        laser.SetActive(true);
        laserRenderer.material = inactiveMaterial;
        laserRenderer.material.color = new Color(color.r, color.g, color.b, 0.4f);
        Vector3 endPos = trackedObj.transform.position + transform.forward * 10f;
        laser.transform.position = Vector3.Lerp(trackedObj.transform.position, endPos, 0.5f);
        laser.transform.LookAt(endPos);
        laser.transform.localScale = new Vector3(laser.transform.localScale.x, laser.transform.localScale.y,
            10);
    }

    private void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100, mask))
        {
            hitInstance.SetActive(true);
            hitPoint = hit.point;
            hitInstance.transform.position = hitPoint + 0.015f * hit.normal;
            hitInstance.transform.rotation = Quaternion.LookRotation(hit.normal);
            if (Controller.GetPress(SteamVR_Controller.ButtonMask.Trigger))
            {
                hitInstance.transform.localScale = cursorPressedSize;
            }
            else
            {
                hitInstance.transform.localScale = cursorSize;
            }

            if (showAlways || Controller.GetPress(SteamVR_Controller.ButtonMask.Trigger))
            {
                ShowLaser(hit);
            }
            else
            {
                laser.SetActive(false);
            }
        }
        else
        {
            if (showAlways)
            {
                ShowDefaultLaser();
            }
            else
            {
                laser.SetActive(false);
            }
            hitInstance.SetActive(false);
        }
    }

    protected override void OnDisable()
    {
        if (hitInstance != null)
        {
            hitInstance.SetActive(false);
        }
        if (laser != null)
        {
            laser.SetActive(false);
        }
    }
}
