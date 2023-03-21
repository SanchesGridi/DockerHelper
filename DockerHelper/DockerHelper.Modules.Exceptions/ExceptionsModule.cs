using DockerHelper.Core;
using DockerHelper.Modules.Exceptions.ViewModels;
using DockerHelper.Modules.Exceptions.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace DockerHelper.Modules.Exceptions;

public class ExceptionsModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider)
    {
        var regionManager = containerProvider.Resolve<IRegionManager>();
        regionManager.RegisterViewWithRegion(RegionNames.ExceptionsRegion, typeof(ConsoleControl));

        regionManager.RequestNavigate(RegionNames.ExceptionsRegion, nameof(ConsoleControl));
    }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<ConsoleControl, ConsoleControlViewModel>();
    }
}