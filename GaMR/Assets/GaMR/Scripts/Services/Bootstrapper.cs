using i5.Toolkit.Core.ServiceCore;
using UnityEngine;

public class Bootstrapper : BaseServiceBootstrapper
{
    [SerializeField] private GameObject progressOrbsPrefab;

    protected override void RegisterServices()
    {
        ServiceManager.RegisterService(new SceneService());
    }

    protected override void UnRegisterServices()
    {
        ServiceManager.RemoveService<SceneService>();
    }
}
