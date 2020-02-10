﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utility class which guides the construction of meshes
/// Provides helper methods to register vertices and create faces of different shapes
/// </summary>
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
    public int AddVertex(Vector3 coordinates, string name)
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
    /// The indices must exist in the geometry, i.e. they first need to be added using AddVertex()
    /// </summary>
    /// <param name="v1">Index of vertex 1</param>
    /// <param name="v2">Index of vertex 2</param>
    /// <param name="v3">Index of vertex 3</param>
    /// <param name="flipNormal">If set to true, the triangle will face the other way</param>
    public void AddTriangle(int v1, int v2, int v3, bool flipNormal = false)
    {
        if (CheckVertexIndex(v1) && CheckVertexIndex(v2) && CheckVertexIndex(v3))
        {
            if (flipNormal)
            {
                Triangles.Add(v1);
                Triangles.Add(v3);
                Triangles.Add(v2);
            }
            else
            {
                Triangles.Add(v1);
                Triangles.Add(v2);
                Triangles.Add(v3);
            }
        }
    }

    /// <summary>
    /// Adds a quad to the geometry (by adding two triangles)
    /// List the four vertices in clockwise order as seen from the outside
    /// The diagonal will be created between the first and third vertex
    /// The indices must exist in the geometry, i.e. they first need to be added using AddVertex()
    /// </summary>
    /// <param name="v1">Index of vertex 1</param>
    /// <param name="v2">Index of vertex 2</param>
    /// <param name="v3">Index of vertex 3</param>
    /// <param name="v4">Index of vertex 4</param>
    /// /// <param name="flipNormals">If set to true, the quad will face the other way</param>
    public void AddQuad(int v1, int v2, int v3, int v4, bool flipNormals = false)
    {
        if (CheckVertexIndex(v1) && CheckVertexIndex(v2) && CheckVertexIndex(v3) && CheckVertexIndex(v4))
        {
            // add two triangles: top right triangle and bottom left triangle
            AddTriangle(v1, v2, v3, flipNormals);
            AddTriangle(v1, v3, v4, flipNormals);
        }
    }

    /// <summary>
    /// Adds a fan of triangles to the geometry
    /// List the otherVertices clockwise
    /// The indices must exist in the geometry, i.e. they first need to be added using AddVertex()
    /// </summary>
    /// <param name="poleVertex">The pole vertex which is connected to all other vertices of the fan</param>
    /// <param name="otherVertices">The vertices which span the fan</param>
    /// <param name="flipNormals">If set to true, the triangle fan will face the other way</param>
    public void AddTriangleFan(int poleVertex, int[] otherVertices, bool flipNormals = false)
    {
        if (CheckVertexIndex(poleVertex))
        {
            // check if all vertices are valid
            for (int i=0;i<otherVertices.Length;i++)
            {
                if (!CheckVertexIndex(otherVertices[i]))
                {
                    return;
                }
            }

            for (int i=0;i<otherVertices.Length-1;i++)
            {
                AddTriangle(poleVertex, otherVertices[i], otherVertices[i + 1], flipNormals);
            }
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
