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

        // vertex positions
        // calculate positions of inner four vertices
        Vector3 leftTopInner = new Vector3(-width / 2f + cornerRadius, height / 2f - cornerRadius, 0);
        Vector3 leftBottomInner = new Vector3(-width / 2f + cornerRadius, -height / 2f + cornerRadius, 0);
        Vector3 rightTopInner = new Vector3(width / 2f - cornerRadius, height / 2f - cornerRadius, 0);
        Vector3 rightBottomInner = new Vector3(width / 2f - cornerRadius, -height / 2f + cornerRadius, 0);

        // calculate positions of outer vertices (not part of the rounded corners)
        Vector3 leftTopOuterLeft = leftTopInner - new Vector3(cornerRadius, 0, 0);
        Vector3 leftTopOuterTop = leftTopInner + new Vector3(0, cornerRadius, 0);
        Vector3 leftBottomOuterLeft = leftBottomInner - new Vector3(cornerRadius, 0, 0);
        Vector3 leftBottomOuterBottom = leftBottomInner - new Vector3(0, cornerRadius, 0);
        Vector3 rightTopOuterRight = rightTopInner + new Vector3(cornerRadius, 0, 0);
        Vector3 rightTopOuterTop = rightTopInner + new Vector3(0, cornerRadius, 0);
        Vector3 rightBottomOuterRight = rightBottomInner + new Vector3(cornerRadius, 0, 0);
        Vector3 rightBottomOuterBottom = rightBottomInner - new Vector3(0, cornerRadius, 0);

        // create the areas twice: once for the front and once for the back of the menu
        for (int i = 0; i < 2; i++)
        {
            Vector3 depthOffset = new Vector3(0, 0, i * depth);

            // get indices for inner four vertices
            int iLeftTopInner = constructor.AddVertex(leftTopInner + depthOffset);
            int iLeftBottomInner = constructor.AddVertex(leftBottomInner + depthOffset);
            int iRightTopInner = constructor.AddVertex(rightTopInner + depthOffset);
            int iRightBottomInner = constructor.AddVertex(rightBottomInner + depthOffset);

            // get indices for outer vertices
            int iLeftTopOuterLeft = constructor.AddVertex(leftTopOuterLeft + depthOffset);
            int iLeftTopOuterTop = constructor.AddVertex(leftTopOuterTop + depthOffset);
            int iLeftBottomOuterLeft = constructor.AddVertex(leftBottomOuterLeft + depthOffset);
            int iLeftBottomOuterBottom = constructor.AddVertex(leftBottomOuterBottom + depthOffset);
            int iRightTopOuterRight = constructor.AddVertex(rightTopOuterRight + depthOffset);
            int iRightTopOuterTop = constructor.AddVertex(rightTopOuterTop + depthOffset);
            int iRightBottomOuterRight = constructor.AddVertex(rightBottomOuterRight + depthOffset);
            int iRightBottomOuterBottom = constructor.AddVertex(rightBottomOuterBottom + depthOffset);

            bool isBackFace = (i == 1);

            // create inner quad
            constructor.AddQuad(iLeftTopInner, iRightTopInner, iRightBottomInner, iLeftBottomInner, isBackFace);
            // create outer border
            constructor.AddQuad(iLeftTopOuterTop, iRightTopOuterTop, iRightTopInner, iLeftTopInner, isBackFace);
            constructor.AddQuad(iRightTopInner, iRightTopOuterRight, iRightBottomOuterRight, iRightBottomInner, isBackFace);
            constructor.AddQuad(iLeftBottomInner, iRightBottomInner, iRightBottomOuterBottom, iLeftBottomOuterBottom, isBackFace);
            constructor.AddQuad(iLeftTopOuterLeft, iLeftTopInner, iLeftBottomInner, iLeftBottomOuterLeft, isBackFace);

            // create the rounded corners
            CreateCorner(constructor, iLeftTopInner, iLeftTopOuterLeft, iLeftTopOuterTop, 0f, isBackFace);
            CreateCorner(constructor, iRightTopInner, iRightTopOuterTop, iRightTopOuterRight, 90f, isBackFace);
            CreateCorner(constructor, iRightBottomInner, iRightBottomOuterRight, iRightBottomOuterBottom, 180f, isBackFace);
            CreateCorner(constructor, iLeftBottomInner, iLeftBottomOuterBottom, iLeftBottomOuterLeft, 270f, isBackFace);
        }

        // create rim vertex indices
        // these vertices need to be separate from the ones above, even if they have the same coordinates to create sharp edges
        int[] rimLeftTopOuterLeft = new int[2];
        int[] rimLeftTopOuterTop = new int[2];
        int[] rimLeftBottomOuterLeft = new int[2];
        int[] rimLeftBottomOuterBottom = new int[2];
        int[] rimRightTopOuterRight = new int[2];
        int[] rimRightTopOuterTop = new int[2];
        int[] rimRightBottomOuterRight = new int[2];
        int[] rimRightBottomOuterBottom = new int[2];

        for (int i = 0; i < 2; i++)
        {
            Vector3 depthOffset = new Vector3(0, 0, i * depth);

            rimLeftTopOuterLeft[i] = constructor.AddVertex(leftTopOuterLeft + depthOffset);
            rimLeftTopOuterTop[i] = constructor.AddVertex(leftTopOuterTop + depthOffset);
            rimLeftBottomOuterLeft[i] = constructor.AddVertex(leftBottomOuterLeft + depthOffset);
            rimLeftBottomOuterBottom[i] = constructor.AddVertex(leftBottomOuterBottom + depthOffset);
            rimRightTopOuterRight[i] = constructor.AddVertex(rightTopOuterRight + depthOffset);
            rimRightTopOuterTop[i] = constructor.AddVertex(rightTopOuterTop + depthOffset);
            rimRightBottomOuterRight[i] = constructor.AddVertex(rightBottomOuterRight + depthOffset);
            rimRightBottomOuterBottom[i] = constructor.AddVertex(rightBottomOuterBottom + depthOffset);
        }

        // top rim
        constructor.AddQuad(rimLeftTopOuterTop[1], rimRightTopOuterTop[1], rimRightTopOuterTop[0], rimLeftTopOuterTop[0]);
        // right rim
        constructor.AddQuad(rimRightTopOuterRight[0], rimRightTopOuterRight[1], rimRightBottomOuterRight[1], rimRightBottomOuterRight[0]);
        // bottom rim
        constructor.AddQuad(rimLeftBottomOuterBottom[0], rimRightBottomOuterBottom[0], rimRightBottomOuterBottom[1], rimLeftBottomOuterBottom[1]);
        // left rim
        constructor.AddQuad(rimLeftTopOuterLeft[1], rimLeftTopOuterLeft[0], rimLeftBottomOuterLeft[0], rimLeftBottomOuterLeft[1]);

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
