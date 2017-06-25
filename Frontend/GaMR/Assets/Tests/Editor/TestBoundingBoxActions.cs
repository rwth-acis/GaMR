using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

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
        actions = go.AddComponent<BoundingBoxActions>();
        actions.boundingBoxPieces = new System.Collections.Generic.List<Transform>();
        boundingBoxPiece = new GameObject().transform;
        actions.boundingBoxPieces.Add(boundingBoxPiece);

        actions.Start();
    }

    [Test]
    public void TestToggleBoundingBox()
    {
        actions.ToggleBoundingBox();
        Assert.IsFalse(coll.enabled);
        Assert.IsFalse(boundingBoxPiece.gameObject.activeSelf);

        actions.ToggleBoundingBox();
        Assert.IsTrue(coll.enabled);
        Assert.IsTrue(boundingBoxPiece.gameObject.activeSelf);
    }
}
