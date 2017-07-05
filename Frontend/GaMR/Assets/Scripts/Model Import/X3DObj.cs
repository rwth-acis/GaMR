using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// organizes a X3D object
/// </summary>
public class X3DObj
{
    private List<X3DPiece> pieces;
    private RestManager restManager;
    private string baseUrl;
    private string url;
    private string name;
    private GameObject parent;
    private Shader shader;
    private System.Action callback;
    public Bounds parentBounds;

    public X3DObj()
    {

    }

    /// <summary>
    /// Constructor to initialize the X3D object's parameters
    /// </summary>
    /// <param name="restManager">The RestManager-component for downloading data</param>
    /// <param name="baseUrl">The base url where the models can be found</param>
    /// <param name="name">The name of the X3D object</param>
    /// <param name="shader">The shader which will be applied to the instantiated gameobject</param>
    public X3DObj(RestManager restManager, string baseUrl, string name, Shader shader)
    {
        pieces = new List<X3DPiece>();
        this.restManager = restManager;
        this.baseUrl = baseUrl;
        this.shader = shader;
        this.name = name;
        url = baseUrl + name + "/";
    }

    /// <summary>
    /// Downloads the gameobjects
    /// When finished it calls the callback-method
    /// </summary>
    /// <param name="callback">Method which is called when the download has finished</param>
    public void LoadGameObjects(System.Action callback)
    {
        this.callback = callback;
        // only get the first piece of the X3D object
        // the first piece contains information if there are successive pieces
        restManager.GET(url + "0", OnFinished);
    }

    /// <summary>
    /// Called when the gameobjects have been loaded
    /// </summary>
    /// <param name="requestResult">The resulting JSON string of the web request;
    /// it contains the information about the X3D piece</param>
    private void OnFinished(string requestResult)
    {
        // de-serialize the json string
        X3DPiece obj = JsonUtility.FromJson<X3DPiece>(requestResult);
        obj.ModelName = name;
        pieces.Add(obj);

        // continue loading models if it is not the last one
        if (obj.PieceIndex < obj.PieceCount - 1)
        {
            restManager.GET(url + (obj.PieceIndex + 1), OnFinished);
        }
        else
        {
            // if it is the last one => create the gameobject
            callback();
        }
    }

    /// <summary>
    /// Creates Gameobjects from the pieces of the X3DObject
    /// </summary>
    /// <returns>The parent GameObject which contains all pieces as children</returns>
    public GameObject CreateGameObjects()
    {
        // create the parent object
        parent = new GameObject("X3D Parent");
        ObjectInfo objInfo = parent.AddComponent<ObjectInfo>();
        objInfo.ModelName = ModelName;

        AnnotationManager annotationManger = parent.AddComponent<AnnotationManager>();

        parentBounds = new Bounds();
        foreach(X3DPiece piece in pieces)
        {
            List<GameObject> pieceObjects = piece.CreateGameObject(shader);
            foreach (GameObject subObject in pieceObjects)
            {
                Renderer renderer = subObject.GetComponent<Renderer>();
                parentBounds.Encapsulate(renderer.bounds);
                // assign the parent
                subObject.transform.parent = parent.transform;
                // assign the TapNotifier and tell it to notify the parent
                TapNotifier tapNotifier = subObject.AddComponent<TapNotifier>();
                tapNotifier.RegisterListenerOnInputDown(annotationManger.TapOnModel);
            }
        }


        // normalize size to fit height of one unit
        float factor = 1 / parentBounds.size.z;


        parent.transform.localScale = new Vector3(
            parent.transform.localScale.x * factor,
            parent.transform.localScale.y * factor,
            parent.transform.localScale.z * factor);


        // reset any offset so that the object is centered
        parent.transform.localPosition -= parentBounds.center * factor;

        return parent;
    }

    /// <summary>
    /// the number of individual pieces in the X3D object
    /// </summary>
    public int PieceCount
    {
        get { return pieces.Count; }
    }

    /// <summary>
    /// The combined bounds of all pieces
    /// This should be used instead of the bounds of the parentObject since they do not consider child-bounds
    /// </summary>
    public Bounds Bounds
    {
        get { return parentBounds; }
    }

    /// <summary>
    /// The name of the 3D model
    /// </summary>
    public string ModelName
    {
        get { return name; }
    }
}
