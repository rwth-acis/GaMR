using UnityEngine.SceneManagement;

namespace i5.GaMR.Tests.PlayMode
{
    public static class PlayModeTestHelper
    {
        private const int testSceneBuildIndex = 3;

        public static void LoadTestScene()
        {
            SceneManager.LoadScene(testSceneBuildIndex);
        }
    }
}
