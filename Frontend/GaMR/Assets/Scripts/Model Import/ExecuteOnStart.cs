using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ExecuteOnStart : MonoBehaviour {

    public Shader shader;
    public string baseUrl = "/resources/model/";
    RestManager restCaller;
    X3DObj x3dObject;
    public GameObject boundingBoxPrefab;
    public Vector3 spawnPosition;
    public Vector3 spawnEulerAngles;
    private InformationManager infoManager;

    // Use this for initialization
    void Start () {
        infoManager = GameObject.Find("InformationManager").GetComponent<InformationManager>();
        restCaller = GetComponent<RestManager>();
        x3dObject = new X3DObj(restCaller, infoManager.BackendAddress + baseUrl, shader);
        x3dObject.LoadGameObjects(OnFinished); // this automatically creates them
    }

    private void OnFinished()
    {
        GameObject obj = x3dObject.CreateGameObjects();
        CreateBoundingBox(obj, x3dObject.Bounds);
    }

    void CreateBoundingBox(GameObject content, Bounds bounds)
    {
        // create a bounding-box around the object
        GameObject box = Instantiate(boundingBoxPrefab);
        box.transform.localScale = bounds.size * (1/bounds.size.z);
        Transform contentHolder = box.transform.Find("Content");
        content.transform.parent = contentHolder;

        // set the correct position
        box.transform.localPosition = spawnPosition;
        box.transform.localRotation = Quaternion.Euler(spawnEulerAngles);
    }
}