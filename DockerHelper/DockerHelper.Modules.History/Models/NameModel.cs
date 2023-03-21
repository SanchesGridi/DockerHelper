namespace DockerHelper.Modules.History.Models;

public class NameModel : EntryModel
{
    private readonly string _name;

    public NameModel(string name) => _name = name;

    public string GetName() => _name;
}
