using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSpawner : MonoBehaviour
{

    [SerializeField]
    private GameObject spawnMenu;

    // Use this for initialization
    void Start()
    {
        GameObject menuInstance = Instantiate(spawnMenu);
        CirclePositioner positioner = menuInstance.GetComponentInChildren<CirclePositioner>();
        BoundingBoxMenu menu = menuInstance.GetComponent<BoundingBoxMenu>();
        menu.BoundingBox = transform.parent.gameObject;
        positioner.boundingBox = transform;
    }
}
