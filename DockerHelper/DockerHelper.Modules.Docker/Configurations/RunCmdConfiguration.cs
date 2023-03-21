#nullable enable

using System.Collections.Generic;

namespace DockerHelper.Modules.Docker.Configurations;

public class RunCmdConfiguration
{
    private readonly string _image;
    private readonly bool _autoRemove;
    private readonly List<VolumeConfiguration> _volumes;
    private readonly List<PortPairConfiguration> _ports;
    private readonly List<EnvVarConfiguration> _envs;

    public string Image => _image;
    public bool AutoRemove => _autoRemove;

    public string? UserName { get; set; }
    public string? ContainerName { get; set; }
    public bool Attach { get; set; }

    public RunCmdConfiguration(string image, bool autoRemove)
    {
        _image = image;
        _autoRemove = autoRemove;

        _volumes = new();
        _ports = new();
        _envs = new();
    }

    public void AddRange(List<VolumeConfiguration> volumes)
    {
        if (volumes?.Count > 0)
        {
            _volumes.AddRange(volumes);
        }
    }

    public void AddRange(List<PortPairConfiguration> ports)
    {
        if (ports?.Count > 0)
        {
            _ports.AddRange(ports);
        }
    }

    public void AddRange(List<EnvVarConfiguration> envs)
    {
        if (envs?.Count > 0)
        {
            _envs.AddRange(envs);
        }
    }

    public List<VolumeConfiguration> GetVolumes() => _volumes;

    public List<PortPairConfiguration> GetPorts() => _ports;

    public List<EnvVarConfiguration> GetEnvs() => _envs;

    public int GetPortPairsCount() => _ports.Count;
}
