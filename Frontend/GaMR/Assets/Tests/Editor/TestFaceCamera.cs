using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class TestFaceCamera {

    GameObject go;
    FaceCamera fc;

    [SetUp]
    public void SetUp()
    {
        go = new GameObject();
        fc = go.AddComponent<FaceCamera>();
        Camera.main.transform.position = new Vector3(0, 1, 0);
        go.transform.position = Vector3.zero;
    }

	[Test]
    public void TestUpdate()
    {
        fc.Update();

        Assert.AreEqual(new Vector3(90, 0, 0), go.transform.rotation.eulerAngles);
    }
}
