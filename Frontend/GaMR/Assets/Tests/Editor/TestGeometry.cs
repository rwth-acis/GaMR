using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class TestGeometry
{

    GameObject obj;

    [SetUp]
    public void SetUp()
    {
        obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
    }

    [Test]
    public void TestGetBoundsIndependentFromRotation_NoRotation()
    {
        obj.transform.rotation = Quaternion.identity;
        Bounds bounds = Geometry.GetBoundsIndependentFromRotation(obj.transform);
        Vector3 expectedSize = new Vector3(1, 1, 1);
        Assert.AreEqual(bounds.size, expectedSize);
    }


    [Test]
    public void TestGetBoundsIndependentFromRotation_Rotated()
    {
        obj.transform.Rotate(Vector3.up, 45);
        Bounds bounds = Geometry.GetBoundsIndependentFromRotation(obj.transform);
        Vector3 expectedSize = new Vector3(1, 1, 1);
        Assert.AreEqual(bounds.size, expectedSize);
    }


}
