using Docker.DotNet.Models;

namespace DockerHelper.Modules.Docker.Extenisons;

public static class DockerParametersExtensions
{
    public static void Attach(this CreateContainerParameters @this)
    {
        @this.AttachStdin = @this.AttachStdout = @this.AttachStderr = true;
    }
}
