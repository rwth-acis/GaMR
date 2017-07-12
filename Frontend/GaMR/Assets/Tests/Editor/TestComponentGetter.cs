using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class TestComponentGetter {

	[SetUp]
    public void SetUp()
    {
        GameObject go = new GameObject("InformationManager");
        go.AddComponent<InformationManager>();
    }

    [Test]
    public void TestGetComponentOnGameobject_ObjectNull()
    {
        InformationManager infoManager = ComponentGetter.GetComponentOnGameobject<InformationManager>("This object does not exist");
        Assert.IsNull(infoManager);
    }

    [Test]
    public void TestGetComponentOnGameobject_()
    {
        InformationManager infoManager = ComponentGetter.GetComponentOnGameobject<InformationManager>("This object does not exist");
        Assert.IsNull(infoManager);
    }
}
