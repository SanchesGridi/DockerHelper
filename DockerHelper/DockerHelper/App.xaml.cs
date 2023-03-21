using DockerHelper.Core.Services;
using DockerHelper.Core.Services.Implementations;
using DockerHelper.Core.Services.Interfaces;
using DockerHelper.Modules.Docker;
using DockerHelper.Modules.Exceptions;
using DockerHelper.Modules.History;
using DockerHelper.ViewModels;
using DockerHelper.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;

namespace DockerHelper;

public partial class App
{
    protected override Window CreateShell()
    {
        return Container.Resolve<MainWindow>();
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<MainWindow, MainWindowViewModel>();

        containerRegistry.RegisterSingleton<IViewProvider, ViewProvider>();
        containerRegistry.RegisterSingleton<IMessageBoxService, MessageBoxService>();
        containerRegistry.RegisterSingleton<IFolderService, FolderService>();
        containerRegistry.RegisterSingleton<ViewHelper>();
    }

    protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
        base.ConfigureModuleCatalog(moduleCatalog);

        moduleCatalog.AddModule<DockerModule>();
        moduleCatalog.AddModule<ExceptionsModule>();
        moduleCatalog.AddModule<HistoryModule>();
    }
}
