using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeometryConstructor
{
    public List<Vector3> Vertices { get; private set; }
    public List<int> Triangles { get; private set; }

    public Dictionary<string, int> NamedVertices { get; private set; }

    public GeometryConstructor()
    {
        Vertices = new List<Vector3>();
        Triangles = new List<int>();
        NamedVertices = new Dictionary<string, int>();
    }

    public int AddVertex(Vector3 coordinates)
    {
        Vertices.Add(coordinates);
        return Vertices.Count - 1;
    }

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

    public void AddTriangle(int v1, int v2, int v3)
    {
        Triangles.Add(v1);
        Triangles.Add(v2);
        Triangles.Add(v3);
    }

    public void AddQuad(int v1, int v2, int v3, int v4)
    {
        AddTriangle(v1, v2, v3);
        AddTriangle(v1, v3, v4);
    }

    public Mesh ConstructMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = Vertices.ToArray();
        mesh.triangles = Triangles.ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }
}
