using System.Collections.Generic;

namespace DockerHelper.Modules.Docker.Utils;

public static class EnvVars
{
    private readonly static string[] _aspKeys = new[]
    {
        "ASPNETCORE_ENVIRONMENT",
        "ASPNETCORE_URLS",
        "ASPNETCORE_HTTPS_PORT",
        "ASPNETCORE_Kestrel__Certificates__Default__Path",
        "ASPNETCORE_Kestrel__Certificates__Default__Password"
    };

    public static List<string> AspList() => new(_aspKeys);
}
