using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeometryConstructor
{
    /// <summary>
    /// The vertices of the geometry object
    /// </summary>
    public List<Vector3> Vertices { get; private set; }
    /// <summary>
    /// The triangles/faces of the geometry object
    /// </summary>
    public List<int> Triangles { get; private set; }

    /// <summary>
    /// A set of vertices which can be named and referenced by their name using this dictionary
    /// </summary>
    public Dictionary<string, int> NamedVertices { get; private set; }

    /// <summary>
    /// Creates the geometry constructor to buid the mesh data
    /// You can only add geometry, not remove it
    /// </summary>
    public GeometryConstructor()
    {
        Vertices = new List<Vector3>();
        Triangles = new List<int>();
        NamedVertices = new Dictionary<string, int>();
    }

    /// <summary>
    /// Adds a disconnected, unnamed vertex to the geometry
    /// </summary>
    /// <param name="coordinates">The coordinates of the vertex</param>
    /// <returns>The index of the created vertex</returns>
    public int AddVertex(Vector3 coordinates)
    {
        Vertices.Add(coordinates);
        return Vertices.Count - 1;
    }

    /// <summary>
    /// Adds a disconnected, named vertex to the geometry
    /// </summary>
    /// <param name="coordinates">The coordinates of the vertex position</param>
    /// <param name="name">The unique name identified which should be assigned to the vertex</param>
    /// <returns>The index of the created vertex</returns>
    public int AddNamedVertex(Vector3 coordinates, string name)
    {
        if (NamedVertices.ContainsKey(name))
        {
            if (Vertices[NamedVertices[name]].Equals(coordinates))
            {
                return NamedVertices[name];
            }
            else
            {
                Debug.LogError("The vertex name already exists and has another coordinate");
                return -1;
            }
        }
        else
        {
            int index = AddVertex(coordinates);
            NamedVertices.Add(name, index);
            return index;
        }
    }

    /// <summary>
    /// Adds a triangle to the geometry
    /// List the three vertices in clockwise order as seen from the outside
    /// The indices must exist in the geometry
    /// </summary>
    /// <param name="v1">Index of vertex 1</param>
    /// <param name="v2">Index of vertex 2</param>
    /// <param name="v3">Index of vertex 3</param>
    public void AddTriangle(int v1, int v2, int v3)
    {
        if (CheckVertexIndex(v1) && CheckVertexIndex(v2) && CheckVertexIndex(v3))
        {
            Triangles.Add(v1);
            Triangles.Add(v2);
            Triangles.Add(v3);
        }
    }

    /// <summary>
    /// Adds a quad to the geometry (by adding two triangles)
    /// List the four vertices in clockwise order as seen from the outside
    /// The indices must exist in the geometry
    /// </summary>
    /// <param name="v1">Index of vertex 1</param>
    /// <param name="v2">Index of vertex 2</param>
    /// <param name="v3">Index of vertex 3</param>
    /// <param name="v4">Index of vertex 4</param>
    public void AddQuad(int v1, int v2, int v3, int v4)
    {
        if (CheckVertexIndex(v1) && CheckVertexIndex(v2) && CheckVertexIndex(v3) && CheckVertexIndex(v4))
        {
            // add two triangles: top right triangle and bottom left triangle
            AddTriangle(v1, v2, v3);
            AddTriangle(v1, v3, v4);
        }
    }

    /// <summary>
    /// Builds a mesh from the constructed geometry data
    /// </summary>
    /// <returns>The constructed mesh which is described by these geometry data</returns>
    public Mesh ConstructMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = Vertices.ToArray();
        mesh.triangles = Triangles.ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }

    /// <summary>
    /// Checks if the given vertex index is in the bounds of the vertex array
    /// </summary>
    /// <param name="vertexIndex">The index of the vertex to check</param>
    /// <returns>True if the referenced vertex exists, false otherwise</returns>
    private bool CheckVertexIndex(int vertexIndex)
    {
        if (vertexIndex < 0 || vertexIndex >= Vertices.Count)
        {
            Debug.LogError("Geometry Construction Error: Referenced index is out of mesh vertices bounds: " + vertexIndex);
            return false;
        }
        return true;
    }
}
