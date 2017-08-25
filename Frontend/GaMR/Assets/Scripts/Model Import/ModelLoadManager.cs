using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Loads the X3D models and handles their instantiation
/// </summary>
public class ModelLoadManager : MonoBehaviour
{

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
        WaitCursor.Show();
    }

    /// <summary>
    /// Called when the X3D object has finished loading
    /// creates GameObjects from the X3D object
    /// and also instantiates a bounding box around the imported object
    /// </summary>
    private void OnFinished()
    {
        WaitCursor.Hide();
        GameObject obj = x3dObject.CreateGameObjects();
        CreateBoundingBox(obj, x3dObject.Bounds);

        CreateGamificationGame(x3dObject.ModelName);

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
        box.transform.localScale = bounds.size;
        Transform contentHolder = box.transform.Find("Content");
        content.transform.parent = contentHolder;

        // set the correct position and rotation
        box.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.5f;
        box.transform.localRotation = Quaternion.Euler(spawnEulerAngles);
    }

    private void CreateGamificationGame(string modelName)
    {
        GamificationFramework.Instance.CreateGame(new Game(modelName),
            (createdGame, createcode) =>
            {
                GamificationFramework.Instance.AddUserToGame(modelName, respondeCode =>
                {
                    if (respondeCode != 200)
                    {
                        Debug.Log("Could not add user to the game");
                    }
                    else
                    {
                        Debug.Log("User successfully added to game " + modelName);
                    }
                });


                Badge defaultBadge = new Badge("defaultBadge", "default badge", "a default badge which is assigned if no other badge was selected");
                defaultBadge.Image = (Texture2D)Resources.Load("DefaultBadge");
                // create a default badge
                GamificationFramework.Instance.CreateBadge(modelName, defaultBadge);

                GameAction defaultAction = new GameAction("defaultAction", "default action", "a default action", 0);
                GamificationFramework.Instance.CreateAction(modelName, defaultAction, null);
            }
            );
    }

    private void GameCreated(Game obj, long responseCode)
    {
        if (obj == null)
        {
            Debug.Log("something went wrong: Http code " + responseCode);
        }
        else
        {
            Debug.Log("ok: " + obj.ID);
        }
    }
}
