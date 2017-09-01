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
    /// <param name="textureName">The name of the associated texture</param>
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

    /// <summary>
    /// Creates GameObjects from the specified vertex and texture information
    /// </summary>
    /// <param name="shader">The shader which is applied to the GameObjects</param>
    /// <returns>Usually one gameobject but if the final mesh has more than 65000 vertices
    /// it will split the mesh and return multiple GameObjects which each have one part of the divided mesh.</returns>
    public List<GameObject> CreateGameObject(Shader shader)
    {
        List<GameObject> results = new List<GameObject>();
        // get mesh
        List<Mesh> subMeshes = new List<Mesh>();
        // if it is not textured => use the unmodified imported mesh
        if (string.IsNullOrEmpty(textureName) || textureCoords == null || textureIndex == null ||textureCoords.Length == 0 ||textureIndex.Length == 0)
        {
            subMeshes = CreateMeshes();
        }
        else
        {
            subMeshes = CreateUVMeshes();
        }

        for (int i = 0; i < subMeshes.Count; i++)
        {

            // create a GameObject for each subMesh and add the necessary components to render it
            GameObject gameObject = new GameObject("Imported X3D");
            gameObject.AddComponent<MeshFilter>();
            MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
            renderer.material = new Material(shader);
            // if it has a texture => attach a TextureLoader to download the assigned texture
            if (!string.IsNullOrEmpty(textureName))
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


    /// <summary>
    /// Creates an untextured mesh from the vertex data of the X3DPiece
    /// </summary>
    /// <returns>the Unity mesh with correct normals</returns>
    private List<Mesh> CreateMeshes()
    {
        List<Mesh> res = new List<Mesh>();
        if (vertexCoords.Count > 64998)
        {
            // use mutliple meshes if there are more than 65000 vertices
            for (int j = 0; j < vertexIndex.Length / 64998 + 1; j++)
            {
                int offset = j * 64998;
                int length = Math.Min(64998, vertexIndex.Length - j * 64998);
                int[] newIndex = new int[length];
                Vector3[] newCoordinates = new Vector3[length];

                // unshare the vertices so that the UV-data can be used by Unity
                for (int i = 0; i < length; i++)
                {
                    newIndex[i] = i;
                    newCoordinates[i] = vertexCoords[vertexIndex[i + offset]];
                }

                // create the mesh
                Mesh mesh = new Mesh();
                mesh.vertices = newCoordinates;
                mesh.triangles = newIndex;

                mesh.RecalculateNormals();

                res.Add(mesh);
            }
        }
        else
        {
            // create a mesh and upload the data it
            Mesh mesh = new Mesh();
            mesh.SetVertices(vertexCoords);
            mesh.SetTriangles(vertexIndex, 0);
            mesh.RecalculateNormals();
            res.Add(mesh);
        }

        return res;
    }

    /// <summary>
    /// Creates a mesh with UV-texture data from the vertex and texture data of the X3DPiece
    /// </summary>
    /// <returns>the Unity mesh</returns>
    private List<Mesh> CreateUVMeshes()
    {
        List<Mesh> untexturedMeshes = CreateMeshes();
        List<Mesh> res = new List<Mesh>();

        // first create the untextured mesh in order to get the correct normals
        foreach (Mesh untextured in untexturedMeshes)
        {
            Vector3[] normals = untextured.normals;
            // use mutliple meshes if there are more than 65000 vertices
            for (int j = 0; j < vertexIndex.Length / 64998 + 1; j++)
            {
                int offset = j * 64998;
                int length = Math.Min(64998, vertexIndex.Length - j * 64998);
                int[] newIndex = new int[length];
                Vector3[] newCoordinates = new Vector3[length];
                Vector2[] newUVCoordinates = new Vector2[length];
                Vector3[] newNormals = new Vector3[length];

                // unshare the vertices so that the UV-data can be used by Unity
                for (int i = 0; i < length; i++)
                {
                    newIndex[i] = i;
                    newCoordinates[i] = vertexCoords[vertexIndex[i + offset]];
                    newUVCoordinates[i] = textureCoords[textureIndex[i + offset]];
                    newNormals[i] = normals[vertexIndex[i + offset]];
                }

                // create the mesh
                Mesh mesh = new Mesh();
                mesh.vertices = newCoordinates;
                mesh.uv = newUVCoordinates;
                mesh.triangles = newIndex;

                mesh.normals = newNormals;

                res.Add(mesh);
            }
        }


        return res;
    }

    /// <summary>
    /// The index of the current piece
    /// </summary>
    public int PieceIndex
    {
        get { return pieceIndex; }
    }

    /// <summary>
    /// The number of pieces which are associated to the same X3DObject at this piece
    /// </summary>
    public int PieceCount
    {
        get { return pieceCount; }
    }

    /// <summary>
    /// The name of the parent-X3DObject
    /// </summary>
    public string ModelName { get; set; }



}
