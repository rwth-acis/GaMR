using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSpawner : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        GameObject menuInstance = Instantiate(WindowResources.Instance.ContextMenu);
        CirclePositioner positioner = menuInstance.GetComponentInChildren<CirclePositioner>();
        BoundingBoxMenu menu = menuInstance.GetComponentInChildren<BoundingBoxMenu>();
        menu.BoundingBox = transform.parent.gameObject;
        Debug.Log("Menu: " + menu.name);
        Debug.Log("menu.BoundingBox: " + menu.BoundingBox.name);
        positioner.boundingBox = transform;
    }
}
