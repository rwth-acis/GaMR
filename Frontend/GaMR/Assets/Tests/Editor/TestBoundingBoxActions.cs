using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using HoloToolkit.Unity.InputModule;

public class TestBoundingBoxActions
{

    GameObject go;
    BoundingBoxActions actions;
    BoxCollider coll;
    Transform boundingBoxPiece;

    [SetUp]
    public void SetUp()
    {
        go = new GameObject();
        coll = go.AddComponent<BoxCollider>();
        go.AddComponent<CustomTapToPlace>();
        go.AddComponent<TransformationManager>();
        actions = go.AddComponent<BoundingBoxActions>();
        actions.boundingBoxPieces = new System.Collections.Generic.List<Transform>();
        boundingBoxPiece = new GameObject().transform;
        actions.boundingBoxPieces.Add(boundingBoxPiece);

        actions.Start();
    }

    [Test]
    public void TestToggleBoundingBox()
    {
        actions.EnableBoundingBox(false);
        Assert.IsFalse(coll.enabled);
        Assert.IsFalse(boundingBoxPiece.gameObject.activeSelf);

        actions.EnableBoundingBox(true);
        Assert.IsTrue(coll.enabled);
        Assert.IsTrue(boundingBoxPiece.gameObject.activeSelf);
    }
}
