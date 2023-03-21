using System;

namespace DockerHelper.Modules.Docker.Configurations;

public class PortPairConfiguration
{
    private readonly int _externalPort;
    private readonly int _internalPort;

    public int ExternalPort => _externalPort;
    public int InternalPort => _internalPort;

    public PortPairConfiguration(int externalPort, int internalPort)
    {
        if (externalPort <= 0)
        {
            throw new InvalidOperationException($"External port was: [{externalPort}]!");
        }
        if (internalPort <= 0)
        {
            throw new InvalidOperationException($"Internal port was: [{internalPort}]!");
        }
        _externalPort = externalPort;
        _internalPort = internalPort;
    }
}
