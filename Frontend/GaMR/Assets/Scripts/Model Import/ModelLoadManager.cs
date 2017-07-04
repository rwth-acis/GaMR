using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Loads the X3D models and handles their instantiation
/// </summary>
public class ModelLoadManager : MonoBehaviour {

    public string baseUrl = "/resources/model/";
    public Shader shader;
    public GameObject boundingBoxPrefab;
    public Vector3 spawnEulerAngles;

    private RestManager restCaller;
    private InformationManager infoManager;
    private X3DObj x3dObject;

    /// <summary>
    /// Called when the script is created
    /// Gets the necessary components
    /// </summary>
    public void Start()
    {
        restCaller = ComponentGetter.GetComponentOnGameobject<RestManager>("RestManager");
        infoManager = ComponentGetter.GetComponentOnGameobject<InformationManager>("InformationManager");
    }

    /// <summary>
    /// Call this to load a new X3D object
    /// </summary>
    /// <param name="name">The name of the X3D object</param>
    public void Load(string name)
    {
        x3dObject = new X3DObj(restCaller, infoManager.BackendAddress + baseUrl, name, shader);
        x3dObject.LoadGameObjects(OnFinished); // this automatically creates them
    }

    /// <summary>
    /// Called when the X3D object has finished loading
    /// creates GameObjects from the X3D object
    /// and also instantiates a bounding box around the imported object
    /// </summary>
    private void OnFinished()
    {
        GameObject obj = x3dObject.CreateGameObjects();
        CreateBoundingBox(obj, x3dObject.Bounds);

    }

    /// <summary>
    /// Creates a bounding box around a GameObject
    /// </summary>
    /// <param name="content">The GameObject which should be encompassed by the bounding box</param>
    /// <param name="bounds">The bounds to encompass (usually the GameObject's bounds or
    /// combined bounds with its children)</param>
    void CreateBoundingBox(GameObject content, Bounds bounds)
    {
        // create a bounding-box around the object
        GameObject box = Instantiate(boundingBoxPrefab);
        box.transform.localScale = bounds.size * (1 / bounds.size.z);
        Transform contentHolder = box.transform.Find("Content");
        content.transform.parent = contentHolder;

        // set the correct position and rotation
        box.transform.position = Camera.main.transform.position + new Vector3(0, 0, 3);
        box.transform.localRotation = Quaternion.Euler(spawnEulerAngles);
    }
}
