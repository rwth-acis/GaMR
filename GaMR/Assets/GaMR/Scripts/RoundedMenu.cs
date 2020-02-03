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
        /*0*/ Vector3 leftTopInner = new Vector3(-width / 2f + cornerRadius, height / 2f - cornerRadius, 0);
        /*1*/ Vector3 leftBottomInner = new Vector3(-width / 2f + cornerRadius, -height / 2f + cornerRadius, 0);
        /*2*/Vector3 rightTopInner = new Vector3(width / 2f - cornerRadius, height / 2f - cornerRadius, 0);
        /*3*/Vector3 rightBottomInner = new Vector3(width / 2f - cornerRadius, -height / 2f + cornerRadius, 0);

        /*4*/ Vector3 leftTopOuterLeft = leftTopInner - new Vector3(cornerRadius, 0, 0);
        /*5*/ Vector3 leftTopOuterTop = leftTopInner + new Vector3(0, cornerRadius, 0);
        /*6*/ Vector3 leftBottomOuterLeft = leftBottomInner - new Vector3(cornerRadius, 0, 0);
        /*7*/ Vector3 leftBottomOuterBottom = leftBottomInner - new Vector3(0, cornerRadius, 0);
        /*8*/ Vector3 rightTopOuterRight = rightTopInner + new Vector3(cornerRadius, 0, 0);
        /*9*/ Vector3 rightTopOuterTop = rightTopInner + new Vector3(0, cornerRadius, 0);
        /*10*/ Vector3 rightBottomOuterRight = rightBottomInner + new Vector3(cornerRadius, 0, 0);
        /*11*/ Vector3 rightBottomOuterBottom = rightBottomInner - new Vector3(0, cornerRadius, 0);

        Mesh mesh = new Mesh();
        mesh.vertices = new Vector3[] {
            leftTopInner, leftBottomInner, rightTopInner, rightBottomInner,
            leftTopOuterLeft, leftTopOuterTop, leftBottomOuterLeft, leftBottomOuterBottom,
            rightTopOuterRight, rightTopOuterTop, rightBottomOuterRight, rightBottomOuterBottom
        };
        mesh.triangles = new int[] {
            0, 3, 1, // inner triangle left bottom
            0, 2, 3, // inner triangle right top
            0, 5, 2, // left bottom triangle of top rim
            5, 9, 2, // right top triangle of top rim
            2, 8, 10, // right top trinagle of right rim
            2, 10, 3 // left bottom triangle of right rim
        };
        return mesh;
    }

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = GenerateMesh();
    }

    private void Update()
    {
        meshFilter.mesh = GenerateMesh();
    }
}
