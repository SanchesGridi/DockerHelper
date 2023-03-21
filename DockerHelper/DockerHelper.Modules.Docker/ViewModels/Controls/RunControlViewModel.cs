using DockerHelper.Core.Events;
using DockerHelper.Core.Extensions;
using DockerHelper.Core.Mvvm.ViewModels;
using DockerHelper.Core.Utils;
using DockerHelper.Modules.Docker.Configurations;
using DockerHelper.Modules.Docker.Utils;
using Prism.Commands;
using Prism.Events;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace DockerHelper.Modules.Docker.ViewModels.Controls;

public class RunControlViewModel : ThreadSaveViewModel
{
    private readonly IDialogService _dialogService;
    private readonly IEventAggregator _eventAggregator;
    private readonly List<PortPairConfiguration> _ports;
    private readonly List<VolumeConfiguration> _volumes;
    private readonly List<EnvVarConfiguration> _envs;

    private string _image;

    private bool _autoRemove;
    public bool AutoRemove
    {
        get => _autoRemove;
        set => SetProperty(ref _autoRemove, value);
    }

    private bool _attach;
    public bool Attach
    {
        get => _attach;
        set => SetProperty(ref _attach, value);
    }

    private string _containerName = Consts.Keys.DefaultKey;
    public string ContainerName
    {
        get => _containerName;
        set => SetProperty(ref _containerName, value);
    }

    private string _userName = Consts.Keys.DefaultKey;
    public string UserName
    {
        get => _userName;
        set => SetProperty(ref _userName, value);
    }

    public DelegateCommand ManagePortsCommand { get; }
    public DelegateCommand ManageVolumesCommand { get; }
    public DelegateCommand ManageEnvironmentsCommand { get; }
    public DelegateCommand CopyDockerRunCmdCommand { get; }

    public RunControlViewModel(IDialogService dialogService, IEventAggregator eventAggregator)
    {
        _dialogService = dialogService;
        _eventAggregator = eventAggregator;
        _ports = new();
        _volumes = new();
        _envs = new();

        var performRunEvent = _eventAggregator.GetEvent<PerformRunEvent>();
        var performRunEventToken = performRunEvent.Subscribe(async (image) => {
            _image = image;
            await PerformRunAsync();
        });

        var imageNameChangedEvent = _eventAggregator.GetEvent<ImageNameChangedEvent>();
        var imageNameChangedEventToken = imageNameChangedEvent.Subscribe((image) => {
            _image = image;
            CopyDockerRunCmdCommand.RaiseCanExecuteChanged();
        });

        _events.Add(performRunEvent, performRunEventToken);
        _events.Add(imageNameChangedEvent, imageNameChangedEventToken);

        ManagePortsCommand = new DelegateCommand(ManagePortsCommandExecute);
        ManageVolumesCommand = new DelegateCommand(ManageVolumesCommandExecute);
        ManageEnvironmentsCommand = new DelegateCommand(ManageEnvironmentsCommandExecute);
        CopyDockerRunCmdCommand = new DelegateCommand(CopyDockerRunCmdCommandExecute, () => !_image.IsEmpty());
    }

    private void ManagePortsCommandExecute()
    {
        ShowConfigurationDialog(
            "Ports configuration", Consts.Keys.PortsKey, Consts.Dialogs.PortsDialog, _ports
        );
    }

    private void ManageVolumesCommandExecute()
    {
        ShowConfigurationDialog(
            "Volumes configuration", Consts.Keys.VolumesKey, Consts.Dialogs.VolumesDialog, _volumes
        );
    }

    private void ManageEnvironmentsCommandExecute()
    {
        ShowConfigurationDialog(
            "Environment variables configuration", Consts.Keys.EnvsKey, Consts.Dialogs.EnvironmentsDialog, _envs
        );
    }

    private void CopyDockerRunCmdCommandExecute()
    {
        try
        {
            var configuration = BuildConfiguration();
            var cmd = DockerRun.CommandString.Build(configuration);
            Clipboard.SetText(cmd);
        }
        catch (Exception ex)
        {
            _eventAggregator.GetEvent<ExceptionEvent>().Publish(ex);
        }
    }

    private void ShowConfigurationDialog<TConfiguration>(string title, string key, string dialog, List<TConfiguration> configurations) where TConfiguration : class
    {
        try
        {
            var parameters = new DialogParameters
            {
                { Consts.Keys.TitleKey, title },
                { key, configurations }
            };
            _dialogService.ShowDialog(dialog, parameters, x =>
            {
                if (x.Result == ButtonResult.OK)
                {
                    configurations.Clear();
                    configurations.AddRange(x.Parameters.GetValue<TConfiguration[]>(key));
                }
            });
        }
        catch (Exception ex)
        {
            _eventAggregator.GetEvent<ExceptionEvent>().Publish(ex);
        }
    }

    private RunCmdConfiguration BuildConfiguration()
    {
        var containerName = ContainerName == Consts.Keys.DefaultKey ? DockerConfig.GetContainerName() : ContainerName;
        var userName = UserName == Consts.Keys.DefaultKey ? DockerConfig.GetUserName() : UserName;

        var configuration = new RunCmdConfiguration(_image, AutoRemove)
        {
            Attach = Attach,
            ContainerName = containerName,
            UserName = userName,
        };
        configuration.AddRange(_ports);
        configuration.AddRange(_volumes);
        configuration.AddRange(_envs);
        return configuration;
    }

    private async Task PerformRunAsync()
    {
        // Summary for approaches:
        // 1) not allowed run with empty ports

        var cmd = Consts.Keys.NotExistingCmdKey;
        var containerId = Consts.Keys.NotExistingIdKey;
        var createdResult = false;
        var runningResult = false;

        try
        {
            var configuration = BuildConfiguration();
            (runningResult, containerId, cmd) = await DockerRun.Async(configuration);
            createdResult = containerId != Consts.Keys.NotExistingIdKey;
        }
        catch (Exception ex)
        {
            _eventAggregator.GetEvent<ExceptionEvent>().Publish(ex);
        }
        finally
        {
            _eventAggregator.GetEvent<RunCompletedEvent>().Publish();
            _eventAggregator.GetEvent<RunContainerEvent>().Publish(new(containerId, createdResult, runningResult, cmd));
        }
    }
}
