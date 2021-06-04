using i5.Toolkit.Core.ServiceCore;
using i5.Toolkit.Core.Utilities;
using i5.Toolkit.Core.Utilities.UnityAdapters;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace i5.GaMR.Services
{
    public class SceneService : IService
    {
        private const int loginScene = 1;
        private const int contentScene = 2;

        private int currentlyLoadedSceneIndex = -1;

        private Scene CurrentlyLoadedScene
        {
            get
            {
                if (currentlyLoadedSceneIndex < 0)
                {
                    return default;
                }
                return SceneManagerWrapper.GetSceneByBuildIndex(currentlyLoadedSceneIndex);
            }
        }

        public ISceneManager SceneManagerWrapper { get; set; } = new SceneManagerWrapper();

        public void Initialize(IServiceManager owner)
        {
        }

        public async void Cleanup()
        {
            await UnloadSceneAsync();
        }

        public async Task LoadSceneAsync(SceneType sceneType)
        {
            await UnloadSceneAsync();
            switch (sceneType)
            {
                case SceneType.LOGIN:
                    await LoadScene(loginScene);
                    break;
                case SceneType.CONTENT:
                    await LoadScene(contentScene);
                    break;
                default:
                    i5Debug.LogError("Tried to load an unrecognized scene", this);
                    break;
            }
        }

        private async Task UnloadSceneAsync()
        {
            if (currentlyLoadedSceneIndex >= 0)
            {
                Scene currentlyLoaded = CurrentlyLoadedScene;
                if (currentlyLoaded.isLoaded)
                {
                    await SceneManagerWrapper.UnloadSceneAsync(currentlyLoaded);
                }
            }
        }

        private async Task LoadScene(int index)
        {
            await SceneManagerWrapper.LoadSceneAsync(index, LoadSceneMode.Additive);
            currentlyLoadedSceneIndex = index;
        }
    }

    public enum SceneType
    {
        LOGIN, CONTENT
    }
}