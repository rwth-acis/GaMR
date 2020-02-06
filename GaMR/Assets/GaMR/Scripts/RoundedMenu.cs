using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundedMenu : MonoBehaviour
{
    public float width = 1f;
    public float height = 1f;
    public float depth = 0.01f;
    public float cornerRadius = 0.1f;
    public int subdivisions = 3;

    private MeshFilter meshFilter;

    private Mesh GenerateMesh()
    {
        /*0*/
        Vector3 leftTopInner = new Vector3(-width / 2f + cornerRadius, height / 2f - cornerRadius, 0);
        /*1*/
        Vector3 leftBottomInner = new Vector3(-width / 2f + cornerRadius, -height / 2f + cornerRadius, 0);
        /*2*/
        Vector3 rightTopInner = new Vector3(width / 2f - cornerRadius, height / 2f - cornerRadius, 0);
        /*3*/
        Vector3 rightBottomInner = new Vector3(width / 2f - cornerRadius, -height / 2f + cornerRadius, 0);

        /*4*/
        Vector3 leftTopOuterLeft = leftTopInner - new Vector3(cornerRadius, 0, 0);
        /*5*/
        Vector3 leftTopOuterTop = leftTopInner + new Vector3(0, cornerRadius, 0);
        /*6*/
        Vector3 leftBottomOuterLeft = leftBottomInner - new Vector3(cornerRadius, 0, 0);
        /*7*/
        Vector3 leftBottomOuterBottom = leftBottomInner - new Vector3(0, cornerRadius, 0);
        /*8*/
        Vector3 rightTopOuterRight = rightTopInner + new Vector3(cornerRadius, 0, 0);
        /*9*/
        Vector3 rightTopOuterTop = rightTopInner + new Vector3(0, cornerRadius, 0);
        /*10*/
        Vector3 rightBottomOuterRight = rightBottomInner + new Vector3(cornerRadius, 0, 0);
        /*11*/
        Vector3 rightBottomOuterBottom = rightBottomInner - new Vector3(0, cornerRadius, 0);

        int vertexIndexOffset = 12; // starting index for the procedurally generated triangles

        Vector3[] topRightVertices = new Vector3[subdivisions];
        int[] topRightTriangles = new int[3 * (subdivisions + 1)]; // there is one triangle more than subdivisions
        // create the vertex fan and connect its triangles
        for (int i = 0; i < subdivisions; i++)
        {
            float radianAngle = Mathf.Deg2Rad * (90f / (subdivisions+1)) * (i+1);
            Vector3 circlePoint = rightTopInner + cornerRadius * new Vector3(Mathf.Cos(radianAngle), Mathf.Sin(radianAngle), 0);
            topRightVertices[i] = circlePoint;
            if (i > 0)
            {
                topRightTriangles[3 * i] = 2; // rightTopInner
                topRightTriangles[3 * i + 1] = i + vertexIndexOffset;
                topRightTriangles[3 * i + 2] = (i - 1) + vertexIndexOffset;
            }
        }

        // connect end points of triangle fan
        topRightTriangles[0] = 2;
        topRightTriangles[1] = vertexIndexOffset;
        topRightTriangles[2] = 8;

        topRightTriangles[topRightTriangles.Length - 3] = 2;
        topRightTriangles[topRightTriangles.Length - 2] = 9;
        topRightTriangles[topRightTriangles.Length - 1] = vertexIndexOffset + subdivisions - 1;

        Mesh mesh = new Mesh();
        Vector3[] fixedVertices = new Vector3[] {
            leftTopInner, leftBottomInner, rightTopInner, rightBottomInner,
            leftTopOuterLeft, leftTopOuterTop, leftBottomOuterLeft, leftBottomOuterBottom,
            rightTopOuterRight, rightTopOuterTop, rightBottomOuterRight, rightBottomOuterBottom
        };
        Vector3[] vertices = new Vector3[fixedVertices.Length + topRightVertices.Length];
        fixedVertices.CopyTo(vertices, 0);
        topRightVertices.CopyTo(vertices, fixedVertices.Length);

        mesh.vertices = vertices;

        int[] fixedTriangles = new int[] {
            0, 3, 1, // inner triangle left bottom
            0, 2, 3, // inner triangle right top
            0, 5, 2, // left bottom triangle of top rim
            5, 9, 2, // right top triangle of top rim
            2, 8, 10, // right top triangle of right rim
            2, 10, 3, // left bottom triangle of right rim
            3, 11, 1, // right top triangle of bottom rim
            1, 11, 7, // left bottom triangle of bottom rim
            0, 1, 4, // right top triangle of left rim
            1, 6, 4 // left bottom triangle of bottom rim
        };
        int[] triangles = new int[fixedTriangles.Length + topRightTriangles.Length];
        fixedTriangles.CopyTo(triangles, 0);
        topRightTriangles.CopyTo(triangles, fixedTriangles.Length);

        mesh.triangles = triangles;
        return mesh;
    }

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        //meshFilter.mesh = GenerateMesh();
    }

    private void Update()
    {
        meshFilter.mesh = GenerateMesh();
    }
}
