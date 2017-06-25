using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class TestTransformationManager {

    GameObject go;
    TransformationManager transManager;
    Vector3 minSize;
    Vector3 maxSize;

    [SetUp]
    public void SetUp()
    {
        go = new GameObject();
        transManager = go.AddComponent<TransformationManager>();
        minSize = new Vector3(0.1f, 0.1f, 0.1f);
        maxSize = new Vector3(2, 2, 2);
        transManager.minSize = minSize;
        transManager.maxSize = maxSize;

        go.transform.localScale = new Vector3(1, 1, 1);
    }

	[Test]
	public void TestScale() {

        Vector3 scalingVector = new Vector3(0.5f, 0.5f, 0.5f);
        transManager.Scale(scalingVector);

        Assert.IsTrue(scalingVector.Equals(go.transform.localScale));
	}

    [Test]
    public void TestScale_UnderMinimum()
    {
        Vector3 scalingVector = new Vector3(0.05f, 0.05f, 0.05f);
        transManager.Scale(scalingVector);
        Assert.IsTrue(minSize.Equals(go.transform.localScale));
    }

    [Test]
    public void TestScale_OverMaximum()
    {
        Vector3 scalingVector = new Vector3(3, 3, 3);
        transManager.Scale(scalingVector);
        Assert.IsTrue(maxSize.Equals(go.transform.localScale));
    }

    [Test]
    public void TestRotate_NegativeAngle()
    {
        go.transform.rotation = Quaternion.identity;
        transManager.Rotate(Vector3.up, -10);
        Assert.IsTrue(go.transform.rotation.eulerAngles.Equals(new Vector3(0, 350, 0)));
    }
}
