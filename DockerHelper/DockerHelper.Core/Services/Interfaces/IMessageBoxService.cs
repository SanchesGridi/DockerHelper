using System.Windows;

namespace DockerHelper.Core.Services.Interfaces;

public interface IMessageBoxService
{
    public bool Show(string message, string title = "Message")
    {
        return System.Windows.MessageBox.Show(message, title, MessageBoxButton.YesNo) == MessageBoxResult.Yes;
    }
}
