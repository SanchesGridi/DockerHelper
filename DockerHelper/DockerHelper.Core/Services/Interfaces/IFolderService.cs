namespace DockerHelper.Core.Services.Interfaces;

public interface IFolderService
{
    public string Browse()
    {
        var selectedPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\";
        using var dialog = new FolderBrowserDialog { SelectedPath = selectedPath };
        var result = dialog.ShowDialog();
        return result == DialogResult.OK ? dialog.SelectedPath : string.Empty;
    }
}
