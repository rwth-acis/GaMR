using FakeItEasy;
using i5.GaMR.Services;
using i5.Toolkit.Core.ServiceCore;
using i5.Toolkit.Core.TestHelpers;
using i5.Toolkit.Core.Utilities.UnityAdapters;
using NUnit.Framework;
using System;
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

            sceneService = new SceneService()
            {
                SceneManagerWrapper = A.Fake<ISceneManager>()
            };
            sceneService.Initialize(A.Fake<IServiceManager>());
        }
    }
}
