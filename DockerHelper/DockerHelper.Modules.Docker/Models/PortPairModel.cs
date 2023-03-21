using Prism.Mvvm;

namespace DockerHelper.Modules.Docker.Models;

public class PortPairModel : BindableBase
{
    private int _externalPort;
    public int ExternalPort
    {
        get => _externalPort;
        set => SetProperty(ref _externalPort, value);
    }

    private int _internalPort;
    public int InternalPort
    {
        get => _internalPort;
        set => SetProperty(ref _internalPort, value);
    }
}
