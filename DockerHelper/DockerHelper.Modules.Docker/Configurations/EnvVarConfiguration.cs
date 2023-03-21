using DockerHelper.Core.Extensions;

namespace DockerHelper.Modules.Docker.Configurations;

public class EnvVarConfiguration
{
    private readonly string _key;
    private readonly string _value;

    public string Key => _key;
    public string Value => _value;

    public EnvVarConfiguration(string key, string value)
    {
        _key = !key.IsEmpty() ? key : throw new("Key was empty!");
        _value = !value.IsEmpty() ? value : throw new("Value was empty!");
    }

    public override string ToString() => $"{_key}={_value}";
}
