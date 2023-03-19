using Prism.Services.Dialogs;
using System.Windows;
using System.Windows.Controls;
using DockerHelper.Core.Services.Interfaces;

namespace DockerHelper.Core.Services;

public class ViewHelper
{
    private readonly IViewProvider _viewProvider;

    public ViewHelper(IViewProvider viewProvider)
    {
        _viewProvider = viewProvider;
    }

    public void ScrollConsole(DependencyObject window, string consoleName)
    {
        var console = _viewProvider.GetView<System.Windows.Controls.ListBox>(window, consoleName);
        var lastIndex = console.Items.Count - 1;
        var lastChild = console.Items[lastIndex];
#if DEBUG
        var typeToShow = lastChild.GetType().Name;
        System.Diagnostics.Debug.WriteLine($"ListBox last child type: [{typeToShow}]");
#endif
        console.ScrollIntoView(lastChild);
    }

    public void ScrollDialogConsole(string consoleName)
    {
        var windows = System.Windows.Application.Current.MainWindow.OwnedWindows;
        if (windows != null && windows.Count > 0 && windows[0] is DialogWindow dialogWindow)
        {
            ScrollConsole(dialogWindow, consoleName);
        }
    }

    public void ScrollViewerScrollToEnd(DependencyObject window, string viewerName)
    {
        var viewer = _viewProvider.GetView<ScrollViewer>(window, viewerName);
        viewer.ScrollToEnd();
    }
}
