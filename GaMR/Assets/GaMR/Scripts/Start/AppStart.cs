using i5.Toolkit.Core.ServiceCore;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppStart : MonoBehaviour
{
    private async void Start()
    {
        SceneService sceneService = ServiceManager.GetService<SceneService>();
        await sceneService.LoadLoginSceneAsync();
    }
}
