using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class TestTapNotifier {

    TapNotifier notifier;

	[SetUp]
    public void SetUp()
    {
        GameObject go = new GameObject("TapNotifier");
        notifier = go.AddComponent<TapNotifier>();
    }

    [Test]
    public void TestRegisterOnInputDown()
    {
        notifier.RegisterListenerOnInputDown(Success);

        notifier.OnInputDown(null);

        Assert.Fail();
    }

    [Test]
    public void TestRegisterOnInputUp()
    {
        notifier.RegisterListenerOnInputUp(Success);

        notifier.OnInputUp(null);

        Assert.Fail();
    }

    [Test]
    public void TestUnregisterOnInputDown()
    {
        notifier.RegisterListenerOnInputDown(Fail);
        notifier.UnRegisterListenerOnInputDown(Fail);

        notifier.OnInputDown(null);
    }

    [Test]
    public void TestUnregisterOnInputUp()
    {
        notifier.RegisterListenerOnInputUp(Fail);
        notifier.UnRegisterListenerOnInputUp(Fail);

        notifier.OnInputUp(null);
    }

    private void Success()
    {
        Assert.Pass();
    }

    private void Fail()
    {
        Assert.Fail();
    }
}
