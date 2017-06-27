using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class X3DPiece
{

    [SerializeField]
    private List<Vector3> vertexCoords;
    [SerializeField]
    private Vector2[] textureCoords;
    [SerializeField]
    private int[] vertexIndex;
    [SerializeField]
    private int[] textureIndex;
    [SerializeField]
    private int pieceIndex;
    [SerializeField]
    private int pieceCount;
    [SerializeField]
    private string textureName;

    /// <summary>
    /// Constructor of a X3D object
    /// </summary>
    /// <param name="vertexCoords">The coordinates of the vertices</param>
    /// <param name="textureCoords">The coordinates on the texture (uv-coordinates)</param>
    /// <param name="vertexIndex">The index that determines which vertices form a face</param>
    /// <param name="textureIndex">The index that determines which vertices form a face on the texture</param>
    public X3DPiece(List<Vector3> vertexCoords, Vector2[] textureCoords, int[] vertexIndex, int[] textureIndex, int pieceIndex, int pieceCount, string textureName)
    {
        this.vertexCoords = vertexCoords;
        this.textureCoords = textureCoords;
        this.vertexIndex = vertexIndex;
        this.textureIndex = textureIndex;
        this.pieceIndex = pieceIndex;
        this.pieceCount = pieceCount;
        this.textureName = textureName;
    }

    public List<GameObject> CreateGameObject(Shader shader)
    {
        List<GameObject> results = new List<GameObject>();
        // get mesh
        List<Mesh> subMeshes = new List<Mesh>();
        if (textureName == "")
        {
            subMeshes.Add(CreateMesh());
        }
        else
        {
            subMeshes = CreateUVMeshes();
        }

        for (int i = 0; i < subMeshes.Count; i++)
        {

            // create a GameObject and add the necessary components to render it
            GameObject gameObject = new GameObject("Imported X3D");
            gameObject.AddComponent<MeshFilter>();
            MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
            renderer.material = new Material(shader);
            if (textureName != null)
            {
                TextureLoader loader = gameObject.AddComponent<TextureLoader>();
                loader.textureUrl = textureName;
                loader.modelName = ModelName;
            }
            gameObject.GetComponent<MeshFilter>().mesh = subMeshes[i];
            // it also needs a mesh collider to react to ray casts
            gameObject.AddComponent<MeshCollider>();

            results.Add(gameObject);
        }

        return results;
    }

    private Mesh CreateMesh()
    {
        // create a mesh and upload the data it
        Mesh mesh = new Mesh();
        mesh.SetVertices(vertexCoords);
        mesh.SetTriangles(vertexIndex, 0);
        mesh.RecalculateNormals();
        return mesh;
    }

    private List<Mesh> CreateUVMeshes()
    {
        List<Mesh> res = new List<Mesh>();
        for (int j = 0; j < vertexIndex.Length / 64998 + 1; j++)
        {
            int offset = j * 64998;
            int length = Math.Min(64998, vertexIndex.Length - j * 64998);
            int[] newIndex = new int[length];
            Vector3[] newCoordinates = new Vector3[length];
            Vector2[] newUVCoordinates = new Vector2[length];

            for (int i = 0; i < length; i++)
            {
                newIndex[i] = i;
                newCoordinates[i] = vertexCoords[vertexIndex[i + offset]];
                newUVCoordinates[i] = textureCoords[textureIndex[i + offset]];
            }

            Mesh mesh = new Mesh();
            mesh.vertices = newCoordinates;
            mesh.uv = newUVCoordinates;
            mesh.triangles = newIndex;

            mesh.RecalculateNormals();

            res.Add(mesh);
        }


        return res;
    }

    public int PieceIndex
    {
        get { return pieceIndex; }
    }

    public int PieceCount
    {
        get { return pieceCount; }
    }

    public string ModelName { get; set; }



}
