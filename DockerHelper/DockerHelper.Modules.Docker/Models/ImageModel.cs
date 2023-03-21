using Prism.Mvvm;

namespace DockerHelper.Modules.Docker.Models;

public class ImageModel : BindableBase
{
    private readonly string _id;
    private readonly long _sizeInBytes;

    private string _name;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string SizeInMegabytes => $"Size: {_sizeInBytes / 1_000_000.0} MB";

    public ImageModel(string id, long size)
    {
        _id = id;
        _sizeInBytes = size;
    }

    public string GetId() => _id;
}
