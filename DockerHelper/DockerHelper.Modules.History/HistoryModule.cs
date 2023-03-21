using DockerHelper.Core;
using DockerHelper.Modules.History.ViewModels;
using DockerHelper.Modules.History.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace DockerHelper.Modules.History;

public class HistoryModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider)
    {
        var regionManager = containerProvider.Resolve<IRegionManager>();
        regionManager.RegisterViewWithRegion(RegionNames.HistoryRegion, typeof(ConsoleControl));

        regionManager.RequestNavigate(RegionNames.HistoryRegion, nameof(ConsoleControl));
    }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<ConsoleControl, ConsoleControlViewModel>();
    }
}