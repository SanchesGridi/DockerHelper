using Prism.Mvvm;
using System.Windows.Media;

namespace DockerHelper.Modules.History.Models;

public class EntryModel : BindableBase
{
    private string _message;
    public string Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }

    private Brush _brush;
    public Brush Brush
    {
        get => _brush ??= Brushes.LightGreen;
        set => SetProperty(ref _brush, value);
    }
}
