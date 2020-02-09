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
        GeometryConstructor constructor = new GeometryConstructor();

        // inner four vertices
        int leftTopInner = constructor.AddVertex(new Vector3(-width / 2f + cornerRadius, height / 2f - cornerRadius, 0));
        int leftBottomInner = constructor.AddVertex(new Vector3(-width / 2f + cornerRadius, -height / 2f + cornerRadius, 0));
        int rightTopInner = constructor.AddVertex(new Vector3(width / 2f - cornerRadius, height / 2f - cornerRadius, 0));
        int rightBottomInner = constructor.AddVertex(new Vector3(width / 2f - cornerRadius, -height / 2f + cornerRadius, 0));

        // outer vertices (not part of the rounded corners)
        int leftTopOuterLeft = constructor.AddVertex(constructor.Vertices[leftTopInner] - new Vector3(cornerRadius, 0, 0));
        int leftTopOuterTop = constructor.AddVertex(constructor.Vertices[leftTopInner] + new Vector3(0, cornerRadius, 0));
        int leftBottomOuterLeft = constructor.AddVertex(constructor.Vertices[leftBottomInner] - new Vector3(cornerRadius, 0, 0));
        int leftBottomOuterBottom = constructor.AddVertex(constructor.Vertices[leftBottomInner] - new Vector3(0, cornerRadius, 0));
        int rightTopOuterRight = constructor.AddVertex(constructor.Vertices[rightTopInner] + new Vector3(cornerRadius, 0, 0));
        int rightTopOuterTop = constructor.AddVertex(constructor.Vertices[rightTopInner] + new Vector3(0, cornerRadius, 0));
        int rightBottomOuterRight = constructor.AddVertex(constructor.Vertices[rightBottomInner] + new Vector3(cornerRadius, 0, 0));
        int rightBottomOuterBottom = constructor.AddVertex(constructor.Vertices[rightBottomInner] - new Vector3(0, cornerRadius, 0));

        // inner quad
        constructor.AddQuad(leftTopInner, rightTopInner, rightBottomInner, leftBottomInner);
        // outer rims
        constructor.AddQuad(leftTopOuterTop, rightTopOuterTop, rightTopInner, leftTopInner);
        constructor.AddQuad(rightTopInner, rightTopOuterRight, rightBottomOuterRight, rightBottomInner);
        constructor.AddQuad(leftBottomInner, rightBottomInner, rightBottomOuterBottom, leftBottomOuterBottom);
        constructor.AddQuad(leftTopOuterLeft, leftTopInner, leftBottomInner, leftBottomOuterLeft);

        // create the rounded corners
        CreateCorner(constructor, leftTopInner, leftTopOuterLeft, leftTopOuterTop, 0f);
        CreateCorner(constructor, rightTopInner, rightTopOuterTop, rightTopOuterRight, 90f);
        CreateCorner(constructor, rightBottomInner, rightBottomOuterRight, rightBottomOuterBottom, 180f);
        CreateCorner(constructor, leftBottomInner, leftBottomOuterBottom, leftBottomOuterLeft, 270f);

        return constructor.ConstructMesh();
    }

    private void CreateCorner(GeometryConstructor constructor, int innerVertex, int outerVertex1, int outerVertex2, float angleOffset)
    {
        int lastIndex = -1;
        for (int i = 0; i < subdivisions; i++)
        {
            float angleStep = 90f / (subdivisions + 1) * (i + 1);
            float radianAngle = Mathf.Deg2Rad * (angleStep + angleOffset - 90f); // -90 correction so that 0 degrees offset is for left top corner
            Vector3 circlePoint = constructor.Vertices[innerVertex] + cornerRadius * new Vector3(Mathf.Sin(radianAngle), Mathf.Cos(radianAngle), 0);
            int vertexIndex = constructor.AddVertex(circlePoint);
            if (i == 0)
            {
                constructor.AddTriangle(innerVertex, outerVertex1, vertexIndex);
            }
            else if (i > 0)
            {
                constructor.AddTriangle(innerVertex, lastIndex, vertexIndex);
            }
            lastIndex = vertexIndex;
        }
        constructor.AddTriangle(innerVertex, lastIndex, outerVertex2);
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
