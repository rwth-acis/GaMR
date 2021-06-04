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
            await UnloadSceneAsync((int)sceneType);
            await LoadScene((int)sceneType);
        }

        private async Task UnloadSceneAsync(int nextSceneIndex = -1)
        {
            if (currentlyLoadedSceneIndex >= 0 && nextSceneIndex != currentlyLoadedSceneIndex)
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
            if (index != currentlyLoadedSceneIndex)
            {
                await SceneManagerWrapper.LoadSceneAsync(index, LoadSceneMode.Additive);
                currentlyLoadedSceneIndex = index;
            }
        }
    }

    public enum SceneType
    {
        LOGIN = 1,
        CONTENT = 2
    }
}