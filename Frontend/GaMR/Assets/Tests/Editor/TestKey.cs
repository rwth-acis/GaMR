using UnityEngine;
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
        parent.AddComponent<BoxCollider>();
        go = new GameObject();
        go.transform.parent = parent.transform;
        keyboard.inputField = go.AddComponent<TextMesh>();
        GameObject background = GameObject.CreatePrimitive(PrimitiveType.Cube);
        background.name = "Background";
        background.transform.parent = parent.transform;
        key = go.AddComponent<Key>();
        parent.AddComponent<Keyboard>();
        captionObj = new GameObject("Caption");
        captionObj.transform.parent = go.transform;
        caption = captionObj.AddComponent<TextMesh>();
        InformationManager infoManager = parent.AddComponent<InformationManager>();
        LocalizationManager manager = parent.AddComponent<LocalizationManager>();
        keyboard.Awake();
        // due to localization 50 keys are expected but for simplicity this test case only provides one
        //LogAssert.Expect(LogType.Error, "Keyboard-layout file has the wrong number of keys: Should be 1 but is 50");
        key.Awake();
    }

    [Test]
    public void TestOnKeyPressed_Letter()
    {
        key.keyType = KeyType.LETTER;
        //caption.text = "D";
        key.Letter = "D";

        key.KeyPressed();

        Assert.IsTrue(keyboard.Text.EndsWith("D"));
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
