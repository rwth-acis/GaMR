using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class TestComponentGetter {

    InformationManager originalInfoManager;
    GameObject informationGameobject;

    [SetUp]
    public void SetUp()
    {
        informationGameobject = new GameObject("InformationManager");
        originalInfoManager = informationGameobject.AddComponent<InformationManager>();
    }

    [Test]
    public void TestGetComponentOnGameobject_ObjectNull()
    {
        InformationManager infoManager = ComponentGetter.GetComponentOnGameobject<InformationManager>("This object does not exist");
        Assert.IsNull(infoManager);
    }

    [Test]
    public void TestGetComponentOnGameobject_NoScriptAttached()
    {
        GameObject go = new GameObject("No InfoManager");
        InformationManager infoManger = ComponentGetter.GetComponentOnGameobject<InformationManager>("No InfoManager");
        Assert.IsNull(infoManger);
    }

    [Test]
    public void TestGetComponentOnGameobject()
    {
        InformationManager infoManager = ComponentGetter.GetComponentOnGameobject<InformationManager>("InformationManager");

        Assert.IsNotNull(infoManager);
    }
}
