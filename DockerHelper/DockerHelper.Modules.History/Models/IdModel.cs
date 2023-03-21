namespace DockerHelper.Modules.History.Models;

public class IdModel : EntryModel
{
    private readonly string _id;

    public IdModel(string id) => _id = id;

    public string GetId() => _id;
}
