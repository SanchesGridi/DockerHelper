#nullable enable

using Docker.DotNet;
using Docker.DotNet.Models;
using DockerHelper.Core.Extensions;
using DockerHelper.Modules.Docker.Configurations;
using DockerHelper.Modules.Docker.Extenisons;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DockerHelper.Modules.Docker.Utils;

public static class DockerRun
{
    public static async Task<(bool Result, string Id, string Cmd)> Async(RunCmdConfiguration configuration)
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }
        if (configuration.Image.IsEmpty())
        {
            throw new InvalidOperationException("Image name or id was empty!");
        }
        var count = configuration.GetPortPairsCount();
        if (count < 1)
        {
            throw new InvalidOperationException($"Port pairs count was: [{count}]!");
        }

        var parameters = new CreateContainerParameters {
            Image = configuration.Image,
            User = configuration.UserName,
            HostConfig = new HostConfig {
                AutoRemove = configuration.AutoRemove
            }
        };

        if (!configuration.ContainerName!.IsEmpty())
        {
            parameters.Name = configuration.ContainerName;
        }
        if (configuration.Attach)
        {
            parameters.Attach();
        }
        AddPorts(configuration, parameters);
        AddVolumes(configuration, parameters);
        AddEnvironmentVariables(configuration, parameters);

        using var client = new DockerClientConfiguration(new Uri(DockerConfig.WindowsPipe)).CreateClient();
        var response = await client.Containers.CreateContainerAsync(parameters);
#if DEBUG
        DebugCreatedContainer(response);
#endif
        var result = await client.Containers.StartContainerAsync(response.ID, new ContainerStartParameters());
        return (result, response.ID, CommandString.Build(configuration));
    }

    private static void AddPorts(RunCmdConfiguration configuration, CreateContainerParameters parameters)
    {
        parameters.ExposedPorts = new Dictionary<string, EmptyStruct>();
        parameters.HostConfig.PortBindings = new Dictionary<string, IList<PortBinding>>();
        foreach (var ports in configuration.GetPorts())
        {
            var dockerPort = $"{ports.InternalPort}/tcp";
            var hostPort = ports.ExternalPort.ToString();
            parameters.ExposedPorts.Add(dockerPort, default);
            parameters.HostConfig.PortBindings.Add(dockerPort, new List<PortBinding>
            {
                new PortBinding { HostPort = hostPort }
            });
        }
    }

    private static void AddVolumes(RunCmdConfiguration configuration, CreateContainerParameters parameters)
    {
        var volumes = configuration.GetVolumes();
        if (volumes.Count > 0)
        {
            // 1) this code deletes the volume after the container is stopped
            // containerParameters.Volumes = new Dictionary<string, EmptyStruct>();
            // foreach (var volume in volumes)
            // {
            //     containerParameters.Volumes.Add($"{volume.HostPath}:{volume.ContainerPath}", default);
            // }

            // 2) this code working successfully: platform - WindowsContainers; volumes: named, path-oriented
            parameters.HostConfig.Binds = new List<string>();
            foreach (var volume in volumes)
            {
                parameters.HostConfig.Binds.Add($"{volume.HostPath}:{volume.ContainerPath}");
            }
        }
    }

    private static void AddEnvironmentVariables(RunCmdConfiguration configuration, CreateContainerParameters parameters)
    {
        var variables = configuration.GetEnvs();
        if (variables.Count > 0)
        {
            parameters.Env = new List<string>(variables.Select(x => x.ToString()!));
        }
    }

    private static void DebugCreatedContainer(CreateContainerResponse response)
    {
        Debug.WriteLine($"Conatiner ID: {response.ID}");
        if (response.Warnings.Any())
        {
            var lineNumber = 1;
            Debug.WriteLine("Warnings:");
            foreach (var warning in response.Warnings)
            {
                Debug.WriteLine($"{lineNumber++}) {warning}");
            }
        }
    }

    public static class CommandString
    {
        public static string Build(RunCmdConfiguration configuration)
        {
            var builder = new StringBuilder("docker run ");
            if (configuration.Attach)
            {
                // docker not apply this option, need additional configuration
                builder.Append("-a ");
            }
            else
            {
                builder.Append("-d ");
            }
            foreach (var ports in configuration.GetPorts())
            {
                builder.Append($"-p {ports.ExternalPort}:{ports.InternalPort} ");
            }
            foreach (var env in configuration.GetEnvs())
            {
                builder.Append($"-e {env.ToString()} ");
            }
            foreach (var volume in configuration.GetVolumes())
            {
                builder.Append($"-v {volume.HostPath}:{volume.ContainerPath} ");
            }
            if (configuration.AutoRemove)
            {
                builder.Append("--rm ");
            }
            if (!configuration.ContainerName!.IsEmpty())
            {
                builder.Append($"--name {configuration.ContainerName} ");
            }
            return builder.Append(configuration.Image).ToString();
        }
    }
}
