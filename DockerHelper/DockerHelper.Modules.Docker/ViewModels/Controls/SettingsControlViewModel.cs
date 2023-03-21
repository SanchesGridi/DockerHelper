using DockerHelper.Core.Events;
using DockerHelper.Core.Extensions;
using DockerHelper.Core.Mvvm.ViewModels;
using DockerHelper.Modules.Docker.Services;
using DockerHelper.Modules.Docker.Utils;
using Prism.Commands;
using Prism.Events;

namespace DockerHelper.Modules.Docker.ViewModels.Controls;

public class SettingsControlViewModel : ThreadSaveViewModel
{
    private readonly IProcessInvoker _dockerProcessInvoker;
    private readonly IEventAggregator _eventAggregator;

    private string _defaultExternalPort = DockerConfig.GetExternalPort().ToString();
    public string DefaultExternalPort
    {
        get => _defaultExternalPort;
        set
        {
            SetProperty(ref _defaultExternalPort, value);
            if (int.TryParse(value, out var port))
            {
                DockerConfig.SetExternalPort(port);
            }
        }
    }

    private string _defaultInternalPort = DockerConfig.GetInternalPort().ToString();
    public string DefaultInternalPort
    {
        get => _defaultInternalPort;
        set
        {
            SetProperty(ref _defaultInternalPort, value);
            if (int.TryParse(value, out var port))
            {
                DockerConfig.SetInternalPort(port);
            }
        }
    }

    private string _defaultContainerName = DockerConfig.GetContainerName();
    public string DefaultContainerName
    {
        get => _defaultContainerName;
        set
        {
            SetProperty(ref _defaultContainerName, value);
            if (!value.IsEmpty())
            {
                DockerConfig.SetContainerName(value);
            }
        }
    }

    private string _defaultUserName = DockerConfig.GetUserName();
    public string DefaultUserName
    {
        get => _defaultUserName;
        set
        {
            SetProperty(ref _defaultUserName, value);
            if (!value.IsEmpty())
            {
                DockerConfig.SetUserName(value);
            }
        }
    }

    public DelegateCommand StartDockerDesktopCommand { get; }

    public SettingsControlViewModel(
        IProcessInvoker processInvoker,
        IEventAggregator eventAggregator)
    {
        _dockerProcessInvoker = processInvoker;
        _eventAggregator = eventAggregator;

        var startDockerDesktopEvent = _eventAggregator.GetEvent<DockerDesktopStartEvent>();
        var startDockerDesktopEventToken = startDockerDesktopEvent.Subscribe(() =>
            _dockerProcessInvoker.Invoke()
        );

        _events.Add(startDockerDesktopEvent, startDockerDesktopEventToken);

        StartDockerDesktopCommand = new DelegateCommand(() => _dockerProcessInvoker.Invoke());
    }
}
