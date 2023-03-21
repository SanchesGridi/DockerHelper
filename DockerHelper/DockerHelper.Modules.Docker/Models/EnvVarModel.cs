using Prism.Mvvm;

namespace DockerHelper.Modules.Docker.Models;

public class EnvVarModel : BindableBase
{
    private string _key;
    public string Key
    {
        get => _key;
        set => SetProperty(ref _key, value);
    }

    private string _value;
    public string Value
    {
        get => _value;
        set => SetProperty(ref _value, value);
    }
}
