#nullable enable

using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DockerHelper.Modules.Docker.Utils;

public static class DockerContainers
{
    public static async Task MonitorAsync(IProgress<Message> progress, ContainerEventsParameters? parameters = null, CancellationToken token = default)
    {
        if (progress == null)
        {
            throw new ArgumentNullException(nameof(progress));
        }
        using var client = new DockerClientConfiguration(new Uri(DockerConfig.WindowsPipe)).CreateClient();
        await client.System.MonitorEventsAsync(parameters ??= new(), progress, token);
    }

    public static async Task ForceRemoveAsync(string id)
    {
        using var client = new DockerClientConfiguration(new Uri(DockerConfig.WindowsPipe)).CreateClient();
        await client.Containers.RemoveContainerAsync(id, new ContainerRemoveParameters { Force = true });
    }

    public static async Task<IList<string>> PruneAsync(ContainersPruneParameters? parameters = null)
    {
        using var client = new DockerClientConfiguration(new Uri(DockerConfig.WindowsPipe)).CreateClient();
        var response = await client.Containers.PruneContainersAsync(parameters);
        return response.ContainersDeleted;
    }

    public static class List
    {
        public static async Task<IList<ContainerListResponse>> Async(ContainersListParameters? parameters = null)
        {
            using var client = new DockerClientConfiguration(new Uri(DockerConfig.WindowsPipe)).CreateClient();
            var response = await client.Containers.ListContainersAsync(parameters ??= new() { All = true });
#if DEBUG
            DebugContainersList(response);
#endif
            return response;
        }

        private static void DebugContainersList(IList<ContainerListResponse> response)
        {
            var number = 1;
            foreach (var item in response)
            {
                var output = $"{number++}) ID: {item.ID}; State: {item.State}; Status: {item.Status};";
                var names = $"Names: [{string.Join(", ", item.Names)}]";
                var image = $"ImageID: {item.ImageID}; Image: {item.Image}";
                Debug.WriteLine(output);
                Debug.WriteLine(names);
                Debug.WriteLine(image);
                if (item.Labels.Count > 0)
                {
                    var labels = $"Labels: [{string.Join(", ", item.Labels.Select(x => $"key: {x.Key}; value: {x.Value}"))}]";
                    Debug.WriteLine(labels);
                }
            }
        }
    }
}
