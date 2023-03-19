using DockerHelper.Core.Services.Interfaces;
using System.Windows;

namespace DockerHelper.Core.Services.Implementations;

public sealed class ViewProvider : IViewProvider
{
    public TView GetView<TView>(DependencyObject rootView, string viewName) where TView : DependencyObject
    {
        if (rootView == null)
        {
            throw new ArgumentNullException(nameof(rootView));
        }
        if (viewName == null)
        {
            throw new ArgumentNullException(nameof(viewName));
        }
        try
        {
            var childView = (TView)LogicalTreeHelper.FindLogicalNode(rootView, viewName);
            return childView;
        }
        catch (Exception)
        {
            throw;
        }
    }
}
