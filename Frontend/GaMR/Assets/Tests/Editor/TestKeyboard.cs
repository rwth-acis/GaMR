using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class TestKeyboard {

    GameObject go;
    Keyboard keyboard;

    [SetUp]
    public void SetUp()
    {
        go = new GameObject();
        keyboard = go.AddComponent<Keyboard>();


        keyboard.Awake();
    }

    //[Test]
    //public void TestCancel()
    //{
    //    keyboard.callWithResult = CallCancel;

    //    keyboard.Cancel();
    //}



    private void CallCancel(string res)
    {
        Assert.IsNull(res);
    }

    private void CallAccept(string res)
    {
        Assert.IsNotNull(res);
    }




}
