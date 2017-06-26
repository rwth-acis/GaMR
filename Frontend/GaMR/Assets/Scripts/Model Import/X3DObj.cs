using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class X3DObj
{
    private List<X3DPiece> pieces;
    private RestManager restFactory;
    private string baseUrl;
    private GameObject parent;
    private Shader shader;
    private System.Action callback;
    public Bounds parentBounds;

    public X3DObj()
    {

    }

    public X3DObj(RestManager restFactory, string baseUrl, Shader shader)
    {
        pieces = new List<X3DPiece>();
        this.restFactory = restFactory;
        this.baseUrl = baseUrl;
        this.shader = shader;
    }

    public void LoadGameObjects(System.Action callback)
    {
        this.callback = callback;
        restFactory.GET(baseUrl + "0", OnFinished);
    }

    private void OnFinished(string requestResult)
    {
        X3DPiece obj = JsonUtility.FromJson<X3DPiece>(requestResult);
        pieces.Add(obj);

        // continue loading models if it is not the last one
        if (obj.PieceIndex < obj.PieceCount - 1)
        {
            restFactory.GET(baseUrl + obj.PieceIndex + 1, OnFinished);
        }
        else
        {
            // if it is the last one => create the gameobject
            callback();
        }
    }

    public GameObject CreateGameObjects()
    {
        // create the parent object and give it the correct position
        parent = new GameObject("X3D Parent");
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

    public int PieceCount
    {
        get { return pieces.Count; }
    }

    public Bounds Bounds
    {
        get { return parentBounds; }
    }
}
