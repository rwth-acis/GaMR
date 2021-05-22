using i5.Toolkit.Core.ServiceCore;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Microsoft.MixedReality.Toolkit.Utilities;

public class SceneService : IService
{
    private const int loginScene = 1;
    private const int contentScene = 2;

    private Scene currentlyLoadedScene;

    public void Initialize(IServiceManager owner)
    {
    }

    public async void Cleanup()
    {
        await UnloadSceneAsync();
    }

    public async Task LoadLoginSceneAsync()
    {
        await UnloadSceneAsync();
        await LoadScene(loginScene);
    }

    public async Task LoadContentSceneAsync()
    {
        await UnloadSceneAsync();
        await LoadScene(contentScene);
    }

    private async Task UnloadSceneAsync()
    {
        if (currentlyLoadedScene != null && currentlyLoadedScene.isLoaded)
        {
            await SceneManager.UnloadSceneAsync(currentlyLoadedScene);
        }
    }

    private async Task LoadScene(int index)
    {
        await SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
        currentlyLoadedScene = SceneManager.GetSceneByBuildIndex(index);
    }
}
