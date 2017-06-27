using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelLoadManager : MonoBehaviour {

    public string baseUrl = "/resources/model/";
    public Shader shader;
    public GameObject boundingBoxPrefab;
    public Vector3 spawnPosition;
    public Vector3 spawnEulerAngles;

    private RestManager restCaller;
    private InformationManager infoManager;
    private X3DObj x3dObject;

    public void Start()
    {
        restCaller = ComponentGetter.GetComponentOnGameobject<RestManager>("RestManager");
        infoManager = ComponentGetter.GetComponentOnGameobject<InformationManager>("InformationManager");
    }

    public void Load(string name)
    {
        x3dObject = new X3DObj(restCaller, infoManager.BackendAddress + baseUrl, name, shader);
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
        box.transform.localScale = bounds.size * (1 / bounds.size.z);
        Transform contentHolder = box.transform.Find("Content");
        content.transform.parent = contentHolder;

        // set the correct position
        box.transform.localPosition = spawnPosition;
        box.transform.localRotation = Quaternion.Euler(spawnEulerAngles);
    }
}
