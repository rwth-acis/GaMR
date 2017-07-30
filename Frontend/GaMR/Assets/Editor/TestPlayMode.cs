using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using NSubstitute;

public class TestPlayMode {

 //   private MainMenuActions _menu;
 //   GameObject _go;

 //   [Test]
	//public void TestStart() {
 //       // Use the Assert class to test conditions.
 //       //MainMenuActions menu = Substitute.For<MainMenuActions>();
 //       //menu.Start();
 //       Assert.IsNotNull(_menu.InfoManager);
 //   }

 //   [Test]
 //   public void TestSetIPAddress()
 //   {
 //       Assert.IsNotNull(_menu.gameObject);

 //       string ip = "192.168.178.0";
 //       _menu.SetIPAddress(ip);
 //       Assert.IsTrue(_menu.InfoManager.ipAddressBackend == ip);
 //       Assert.IsTrue(_menu.InfoManager.BackendAddress == "http://" + ip + ":8080");
 //   }

 //   [SetUp]
 //   public void SetUp()
 //   {
 //       //_menu = Substitute.For<MainMenuActions>();
 //       ////        _menu.gameObject = Substitute.For(GameObject);
 //       //_menu.get_gameObject.Returns(new GameObject());

 //       _go = new GameObject();
 //       _menu=_go.AddComponent<MainMenuActions>();
      

 //       _menu.Start();

 //   }

 //   // A UnityTest behaves like a coroutine in PlayMode
 //   // and allows you to yield null to skip a frame in EditMode
 //   [UnityTest]
	//public IEnumerator TestPlayModeWithEnumeratorPasses() {
	//	// Use the Assert class to test conditions.
	//	// yield to skip a frame
	//	yield return null;
	//}
}
