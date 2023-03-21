#nullable enable

using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DockerHelper.Core.Utils;

namespace DockerHelper.Modules.Docker.Utils;

public static class DockerImages
{
    public static class List
    {
        public static async Task<IList<ImagesListResponse>> Async(ImagesListParameters? parameters = null)
        {
            using var client = new DockerClientConfiguration(new Uri(DockerConfig.WindowsPipe)).CreateClient();
            return await client.Images.ListImagesAsync(parameters ??= new());
        }
    }

    public static class Delete
    {
        public static async Task<List<(string, string)>> Async(string image, ImageDeleteParameters? parameters = null)
        {
            using var client = new DockerClientConfiguration(new Uri(DockerConfig.WindowsPipe)).CreateClient();
            var deleted = await client.Images.DeleteImageAsync(image, parameters ??= new());
            var result = new List<(string, string)>();
            foreach (var dictionary in deleted)
            {
                foreach (var pair in dictionary)
                {
                    result.Add((pair.Key, pair.Value));
                }
            }
            result.Insert(0, (Consts.Keys.DeletedImageKey, image));
            return result;
        }
    }
}
