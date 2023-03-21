using Prism.Mvvm;

namespace DockerHelper.Modules.Docker.Models;

public class ContainerModel : BindableBase
{
    private string _id;
    public string Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    private string _name;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }
}
