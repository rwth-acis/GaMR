using FakeItEasy;
using i5.GaMR.Services;
using i5.Toolkit.Core.ServiceCore;
using i5.Toolkit.Core.TestHelpers;
using i5.Toolkit.Core.Utilities.UnityAdapters;
using NUnit.Framework;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace i5.GaMR.Tests.PlayMode
{
    public class SceneServiceTests
    {
        private const int loginSceneIndex = 1;
        private const int contentSceneIndex = 2;

        private SceneService sceneService;

        [SetUp]
        public void Setup()
        {
            PlayModeTestHelper.LoadTestScene();

            sceneService = new SceneService();
            sceneService.Initialize(A.Fake<IServiceManager>());
        }

        [UnityTest]
        public IEnumerator LoadSceneAsync_FirstLoginThenContent_LoginLoadedAndUnloaded()
        {
            Task task = sceneService.LoadSceneAsync(SceneType.LOGIN);
            yield return AsyncTest.WaitForTask(task);

            Scene expectedLogin = SceneManager.GetSceneByBuildIndex(loginSceneIndex);
            Assert.IsTrue(expectedLogin.isLoaded);

            task = sceneService.LoadSceneAsync(SceneType.CONTENT);
            yield return AsyncTest.WaitForTask(task);

            Assert.IsFalse(expectedLogin.isLoaded);
        }

        [UnityTest]
        public IEnumerator LoadSceneAsync_FirstLoginThenContent_ContentLoaded()
        {
            Task task = sceneService.LoadSceneAsync(SceneType.LOGIN);
            yield return AsyncTest.WaitForTask(task);

            task = sceneService.LoadSceneAsync(SceneType.CONTENT);
            yield return AsyncTest.WaitForTask(task);

            Scene expectedContent = SceneManager.GetSceneByBuildIndex(contentSceneIndex);
            Assert.IsTrue(expectedContent.isLoaded);
        }
    }
}
