using Prism.Mvvm;

namespace DockerHelper.Modules.Docker.Models;

public class VolumeModel : BindableBase
{
    private string _hostPath;
    public string HostPath
    {
        get => _hostPath;
        set => SetProperty(ref _hostPath, value);
    }

    private string _containerPath;
    public string ContainerPath
    {
        get => _containerPath;
        set => SetProperty(ref _containerPath, value);
    }
}
