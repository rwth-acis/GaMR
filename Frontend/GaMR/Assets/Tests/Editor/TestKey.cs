﻿using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using NSubstitute;

public class TestKey {

    GameObject parent, go, captionObj;
    TextMesh caption;
    Key key;
    Keyboard keyboard;

	[SetUp]
    public void SetUp()
    {
        parent = new GameObject();
        keyboard = parent.AddComponent<Keyboard>();
        go = new GameObject();
        go.transform.parent = parent.transform;
        keyboard.inputField = go.AddComponent<TextMesh>();
        key = go.AddComponent<Key>();
        parent.AddComponent<Keyboard>();
        captionObj = new GameObject("Caption");
        captionObj.transform.parent = go.transform;
        caption = captionObj.AddComponent<TextMesh>();
        InformationManager infoManager = parent.AddComponent<InformationManager>();
        LocalizationManager manager = parent.AddComponent<LocalizationManager>();
        infoManager.Start();
        manager.Start();
        keyboard.Start();
        // due to localization 50 keys are expected but for simplicity this test case only provides one
        LogAssert.Expect(LogType.Error, "Keyboard-layout file has the wrong number of keys: Should be 1 but is 50");
        key.Start();
    }

    [Test]
    public void TestOnKeyPressed_Letter()
    {
        key.keyType = KeyType.LETTER;
        //caption.text = "D";
        key.Letter = "D";

        key.KeyPressed();

        Assert.AreEqual("D", keyboard.Text);
    }


    [Test]
    public void TestOnKeyPressed_Back_StringNotEmpty()
    {
        key.keyType = KeyType.BACK;
        keyboard.Text = "This is a test";

        key.KeyPressed();

        Assert.AreEqual("This is a tes", keyboard.Text);
    }

    [Test]
    public void TestOnKeyPressed_Back_StringEmpty()
    {
        key.keyType = KeyType.BACK;
        keyboard.Text = "";

        Assert.DoesNotThrow(() => key.KeyPressed());

        Assert.AreEqual("", keyboard.Text);
    }

}
