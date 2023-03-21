using DockerHelper.Core.Extensions;

namespace DockerHelper.Modules.Docker.Utils;

public static class DockerConfig
{
    private const int _defaultPort = 80;
    private const string _defaultContainerName = "default-asp-c";
    private const string _defaultUserName = "ContainerUser";

    private static int _externalPort = 0;
    private static int _internalPort = 0;
    private static string _containerName = string.Empty;
    private static string _userName = string.Empty;

    public const string UnnamedImage = "<none>:<none>";
    public const string EngineException = "Docker API responded with status code=InternalServerError, response=open \\\\.\\pipe\\docker_engine_windows: The system cannot find the file specified.";
    public const string WindowsPipe = "npipe://./pipe/docker_engine"; // "npipe:////./pipe/docker_engine_windows"

    public static int GetExternalPort()
    {
        return _externalPort == 0 ? _defaultPort : _externalPort;
    }

    public static int GetInternalPort()
    {
        return _internalPort == 0 ? _defaultPort : _internalPort;
    }

    public static string GetContainerName()
    {
        return _containerName.IsEmpty() ? _defaultContainerName : _containerName;
    }

    public static string GetUserName()
    {
        return _userName.IsEmpty() ? _defaultUserName : _userName;
    }

    public static void SetExternalPort(int port) => _externalPort = port;

    public static void SetInternalPort(int port) => _internalPort = port;

    public static void SetContainerName(string container) => _containerName = container;

    public static void SetUserName(string user) => _userName = user;
}
