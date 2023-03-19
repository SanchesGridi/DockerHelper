using System.Windows;

namespace DockerHelper.Core.Services.Interfaces;

public interface IViewProvider
{
    TView GetView<TView>(DependencyObject rootView, string viewName) where TView : DependencyObject;
}
