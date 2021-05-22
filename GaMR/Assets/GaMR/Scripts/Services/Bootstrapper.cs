using i5.Toolkit.Core.ServiceCore;

public class Bootstrapper : BaseServiceBootstrapper
{
    protected override void RegisterServices()
    {
        ServiceManager.RegisterService(new SceneService());
    }

    protected override void UnRegisterServices()
    {
        ServiceManager.RemoveService<SceneService>();
    }
}
