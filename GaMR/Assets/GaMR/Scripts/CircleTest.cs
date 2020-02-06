using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleTest : MonoBehaviour
{
    public int subdivisions = 32;
    public float radius = 1f;

    private MeshFilter meshFilter;


    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    private void Update()
    {
        meshFilter.mesh = GenerateMesh();
    }

    private Mesh GenerateMesh()
    {
        Vector3[] vertices = new Vector3[subdivisions + 1]; // +1 for midpoint
        int[] triangles = new int[3 * (subdivisions+1)];

        int vertexIndexOffset = 1;

        for (int i = 0; i < subdivisions; i++)
        {
            float radianAngle = Mathf.Deg2Rad * (360f / subdivisions) * i;
            vertices[i + vertexIndexOffset] = radius * new Vector3(Mathf.Cos(radianAngle), Mathf.Sin(radianAngle), 0);

            if (i > 0)
            {
                triangles[3 * i] = 0; // rightTopInner
                triangles[3 * i + 1] = i + vertexIndexOffset;
                triangles[3 * i + 2] = (i - 1) + vertexIndexOffset;
            }
        }

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = subdivisions;

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        return mesh;
    }
}
