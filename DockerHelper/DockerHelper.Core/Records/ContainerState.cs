namespace DockerHelper.Core.Records;

public record ContainerState(string Id, bool Created, bool Running, string Cmd);
