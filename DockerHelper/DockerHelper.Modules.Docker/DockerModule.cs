using DockerHelper.Core;
using DockerHelper.Core.Utils;
using DockerHelper.Modules.Docker.Services;
using DockerHelper.Modules.Docker.ViewModels.Controls;
using DockerHelper.Modules.Docker.ViewModels.Dialogs;
using DockerHelper.Modules.Docker.Views.Controls;
using DockerHelper.Modules.Docker.Views.Dialogs;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace DockerHelper.Modules.Docker;

public class DockerModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider)
    {
        var regionManager = containerProvider.Resolve<IRegionManager>();

        regionManager.RegisterViewWithRegion(RegionNames.DockerContainersRegion, typeof(ContainersControl));
        regionManager.RegisterViewWithRegion(RegionNames.DockerImagesRegion, typeof(ImagesControl));
        regionManager.RegisterViewWithRegion(RegionNames.DockerRunRegion, typeof(RunControl));
        regionManager.RegisterViewWithRegion(RegionNames.DockerSettingsRegion, typeof(SettingsControl));

        regionManager.RequestNavigate(RegionNames.DockerContainersRegion, nameof(ContainersControl));
        regionManager.RequestNavigate(RegionNames.DockerImagesRegion, nameof(ImagesControl));
        regionManager.RequestNavigate(RegionNames.DockerRunRegion, nameof(RunControl));
        regionManager.RequestNavigate(RegionNames.DockerSettingsRegion, nameof(SettingsControl));
    }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<IProcessInvoker, DockerProcessInvoker>();

        containerRegistry.RegisterForNavigation<ContainersControl, ContainersControlViewModel>();
        containerRegistry.RegisterForNavigation<ImagesControl, ImagesControlViewModel>();
        containerRegistry.RegisterForNavigation<RunControl, RunControlViewModel>();
        containerRegistry.RegisterForNavigation<SettingsControl, SettingsControlViewModel>();

        containerRegistry.RegisterDialog<PortsDialogControl, PortsDialogControlViewModel>(Consts.Dialogs.PortsDialog);
        containerRegistry.RegisterDialog<VolumesDialogControl, VolumesDialogControlViewModel>(Consts.Dialogs.VolumesDialog);
        containerRegistry.RegisterDialog<EnvironmentsDialogControl, EnvironmentsDialogControlViewModel>(Consts.Dialogs.EnvironmentsDialog);
    }
}