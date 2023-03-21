namespace DockerHelper.Modules.Docker.Configurations;

public class VolumeConfiguration
{
    private readonly string _hostPath;
    private readonly string _containerPath;

    public string HostPath => _hostPath;
    public string ContainerPath => _containerPath;

    public VolumeConfiguration(string hostPath, string containerPath)
    {
        _hostPath = hostPath;
        _containerPath = containerPath;
    }
}
