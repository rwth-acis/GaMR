using FakeItEasy;
using i5.GaMR.Services;
using i5.Toolkit.Core.ServiceCore;
using i5.Toolkit.Core.TestHelpers;
using i5.Toolkit.Core.Utilities.UnityAdapters;
using NUnit.Framework;
using System.Collections;
using System.Threading.Tasks;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace i5.GaMR.Tests.EditMode.Services
{
    public class SceneServiceTests
    {
        private const int loginSceneIndex = 1;
        private const int contentScentIndex = 2;

        private SceneService sceneService;

        [SetUp]
        public void Setup()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);

            sceneService = new SceneService()
            {
                SceneManagerWrapper = A.Fake<ISceneManager>()
            };
            sceneService.Initialize(A.Fake<IServiceManager>());
        }

        [UnityTest]
        public IEnumerator LoadSceneAsync_Login_LoginSceneLoaded()
        {
            Task task = sceneService.LoadSceneAsync(SceneType.LOGIN);
            yield return AsyncTest.WaitForTask(task);

            A.CallTo(()=>sceneService.SceneManagerWrapper.LoadSceneAsync(
                loginSceneIndex, UnityEngine.SceneManagement.LoadSceneMode.Additive))
                .MustHaveHappened();
        }

        [UnityTest]
        public IEnumerator LoadSceneAsync_LoginNoSceneLoaded_NoSceneUnloaded()
        {
            Task task = sceneService.LoadSceneAsync(SceneType.LOGIN);
            yield return AsyncTest.WaitForTask(task);

            A.CallTo(() => sceneService.SceneManagerWrapper.UnloadSceneAsync(A<Scene>.Ignored)).MustNotHaveHappened();
        }

        [UnityTest]
        public IEnumerator LoadSceneAsync_LoginAndSceneLoaded_SceneUnloaded()
        {
            EditorSceneManager.LoadScene(loginSceneIndex, LoadSceneMode.Additive);
            Scene fakeScene = EditorSceneManager.GetSceneByBuildIndex(loginSceneIndex);
            A.CallTo(() => sceneService.SceneManagerWrapper.GetSceneByBuildIndex(A<int>.Ignored)).Returns(fakeScene);

            Task task = sceneService.LoadSceneAsync(SceneType.LOGIN);
            yield return AsyncTest.WaitForTask(task);

            A.CallTo(() => sceneService.SceneManagerWrapper.UnloadSceneAsync(fakeScene)).MustHaveHappenedOnceExactly();
        }

    }
}
