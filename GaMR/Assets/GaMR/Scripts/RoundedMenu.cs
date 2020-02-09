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

    private int startBackVertices;

    private Mesh GenerateMesh()
    {
        GeometryConstructor constructor = new GeometryConstructor();

        const string nLeftTopOuterLeft = "leftTopOuterLeft";
        const string nleftTopOuterTop = "leftTopOuterTop";
        const string nLeftBottomOuterLeft = "leftBottomOuterLeft";
        const string nLeftBottomOuterBottom = "leftBottomOuterBottom";
        const string nRightTopOuterRight = "rightTopOuterRight";
        const string nRightTopOuterTop = "rightTopOuterTop";
        const string nRightBottomOuterRight = "rightBottomOuterRight";
        const string nRightBottomOuterBottom = "rightBottomOuterBottom";


        for (int i = 0; i < 2; i++)
        {
            float vertexDepth = i * depth;

            // inner four vertices
            int leftTopInner = constructor.AddVertex(new Vector3(-width / 2f + cornerRadius, height / 2f - cornerRadius, vertexDepth));
            int leftBottomInner = constructor.AddVertex(new Vector3(-width / 2f + cornerRadius, -height / 2f + cornerRadius, vertexDepth));
            int rightTopInner = constructor.AddVertex(new Vector3(width / 2f - cornerRadius, height / 2f - cornerRadius, vertexDepth));
            int rightBottomInner = constructor.AddVertex(new Vector3(width / 2f - cornerRadius, -height / 2f + cornerRadius, vertexDepth));

            // outer vertices (not part of the rounded corners)
            int leftTopOuterLeft = constructor.AddVertex(constructor.Vertices[leftTopInner] - new Vector3(cornerRadius, 0, 0), nLeftTopOuterLeft + i);
            int leftTopOuterTop = constructor.AddVertex(constructor.Vertices[leftTopInner] + new Vector3(0, cornerRadius, 0), nleftTopOuterTop + i);
            int leftBottomOuterLeft = constructor.AddVertex(constructor.Vertices[leftBottomInner] - new Vector3(cornerRadius, 0, 0), nLeftBottomOuterLeft + i);
            int leftBottomOuterBottom = constructor.AddVertex(constructor.Vertices[leftBottomInner] - new Vector3(0, cornerRadius, 0), nLeftBottomOuterBottom + i);
            int rightTopOuterRight = constructor.AddVertex(constructor.Vertices[rightTopInner] + new Vector3(cornerRadius, 0, 0), nRightTopOuterRight + i);
            int rightTopOuterTop = constructor.AddVertex(constructor.Vertices[rightTopInner] + new Vector3(0, cornerRadius, 0), nRightTopOuterTop + i);
            int rightBottomOuterRight = constructor.AddVertex(constructor.Vertices[rightBottomInner] + new Vector3(cornerRadius, 0, 0), nRightBottomOuterRight + i);
            int rightBottomOuterBottom = constructor.AddVertex(constructor.Vertices[rightBottomInner] - new Vector3(0, cornerRadius, 0), nRightBottomOuterBottom + i);

            bool isBackFace = (i == 1);

            // inner quad
            constructor.AddQuad(leftTopInner, rightTopInner, rightBottomInner, leftBottomInner, isBackFace);
            // outer border
            constructor.AddQuad(leftTopOuterTop, rightTopOuterTop, rightTopInner, leftTopInner, isBackFace);
            constructor.AddQuad(rightTopInner, rightTopOuterRight, rightBottomOuterRight, rightBottomInner, isBackFace);
            constructor.AddQuad(leftBottomInner, rightBottomInner, rightBottomOuterBottom, leftBottomOuterBottom, isBackFace);
            constructor.AddQuad(leftTopOuterLeft, leftTopInner, leftBottomInner, leftBottomOuterLeft, isBackFace);

            // create the rounded corners
            CreateCorner(constructor, leftTopInner, leftTopOuterLeft, leftTopOuterTop, 0f, isBackFace);
            CreateCorner(constructor, rightTopInner, rightTopOuterTop, rightTopOuterRight, 90f, isBackFace);
            CreateCorner(constructor, rightBottomInner, rightBottomOuterRight, rightBottomOuterBottom, 180f, isBackFace);
            CreateCorner(constructor, leftBottomInner, leftBottomOuterBottom, leftBottomOuterLeft, 270f, isBackFace);

            if (i == 0)
            {
                startBackVertices = constructor.Vertices.Count;
            }
        }

        // create rim
        // top rim
        constructor.AddQuad(
            GetVertexByName(constructor, nleftTopOuterTop, false),
            GetVertexByName(constructor, nRightTopOuterTop, false),
            GetVertexByName(constructor, nRightTopOuterTop, true),
            GetVertexByName(constructor, nleftTopOuterTop, true));

        // right rim
        constructor.AddQuad(
            GetVertexByName(constructor, nRightTopOuterRight, true),
            GetVertexByName(constructor, nRightTopOuterRight, false),
            GetVertexByName(constructor, nRightBottomOuterRight, false),
            GetVertexByName(constructor, nRightBottomOuterRight, true)
            );

        // bottom rim
        constructor.AddQuad(
            GetVertexByName(constructor, nLeftBottomOuterBottom, true),
            GetVertexByName(constructor, nRightBottomOuterBottom, true),
            GetVertexByName(constructor, nRightBottomOuterBottom, false),
            GetVertexByName(constructor, nLeftBottomOuterBottom, false)
            );

        // left rim
        constructor.AddQuad(
            GetVertexByName(constructor, nLeftTopOuterLeft, false),
            GetVertexByName(constructor, nLeftTopOuterLeft, true),
            GetVertexByName(constructor, nLeftBottomOuterLeft, true),
            GetVertexByName(constructor, nLeftBottomOuterLeft, false)
            );
        return constructor.ConstructMesh();
    }

    private int GetVertexByName(GeometryConstructor constructor, string name, bool front)
    {
        if (front)
        {
            return constructor.NamedVertices[name + "0"];
        }
        else
        {
            return constructor.NamedVertices[name + "1"];
        }
    }

    private void CreateCorner(GeometryConstructor constructor, int innerVertex, int outerVertex1, int outerVertex2, float angleOffset, bool isBackFace)
    {
        int lastIndex = -1;
        for (int i = 0; i < subdivisions; i++)
        {
            float angleStep = 90f / (subdivisions + 1) * (i + 1);
            float radianAngle = Mathf.Deg2Rad * (angleStep + angleOffset - 90f); // -90 correction so that 0 degrees offset is for left top corner
            Vector3 circlePoint = circlePoint = constructor.Vertices[innerVertex] + cornerRadius * new Vector3(Mathf.Sin(radianAngle), Mathf.Cos(radianAngle), 0);

            int vertexIndex = constructor.AddVertex(circlePoint);
            if (i == 0)
            {
                constructor.AddTriangle(innerVertex, outerVertex1, vertexIndex, isBackFace);
            }
            else if (i > 0)
            {
                constructor.AddTriangle(innerVertex, lastIndex, vertexIndex, isBackFace);
            }
            lastIndex = vertexIndex;
        }
        constructor.AddTriangle(innerVertex, lastIndex, outerVertex2, isBackFace);
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
