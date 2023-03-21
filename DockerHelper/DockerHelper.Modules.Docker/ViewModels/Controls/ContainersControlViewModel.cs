using Docker.DotNet.Models;
using DockerHelper.Core.Events;
using DockerHelper.Core.Exceptions;
using DockerHelper.Core.Extensions;
using DockerHelper.Core.Mvvm.ViewModels;
using DockerHelper.Core.Services;
using DockerHelper.Core.Services.Interfaces;
using DockerHelper.Core.Utils;
using DockerHelper.Modules.Docker.Enums;
using DockerHelper.Modules.Docker.Models;
using DockerHelper.Modules.Docker.Utils;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DockerHelper.Modules.Docker.ViewModels.Controls;

public class ContainersControlViewModel : ThreadSaveViewModel
{
    private const string MessageBoxTitle = "Docker container event tracking error";

    private readonly IMessageBoxService _messageBoxService;
    private readonly IEventAggregator _eventAggregator;
    private readonly ViewHelper _viewHelper;
    private readonly List<Message> _messages;

    private CancellationTokenSource _monitorCts;

    private Visibility _progressVisibility = Visibility.Hidden;
    public Visibility ProgressVisibility
    {
        get => _progressVisibility;
        set => SetProperty(ref _progressVisibility, value);
    }

    private bool _isContainerOperationsEnabled = true;
    public bool IsContainerOperationsEnabled
    {
        get => _isContainerOperationsEnabled;
        set => SetProperty(ref _isContainerOperationsEnabled, value);
    }


    private ContainerModel _selectedContainer;
    public ContainerModel SelectedContainer
    {
        get => _selectedContainer;
        set
        {
            SetProperty(ref _selectedContainer, value);
            ForceRemoveCommand.RaiseCanExecuteChanged();
        }
    }


    private ObservableCollection<ContainerModel> _containers;
    public ObservableCollection<ContainerModel> Containers
    {
        get => _containers ??= new();
        set => SetProperty(ref _containers, value);
    }

    public DelegateCommand ContainersCommand { get; }
    public DelegateCommand ForceRemoveCommand { get; }
    public DelegateCommand PruneCommand { get; }
    public DelegateCommand CopyContainerEventsCommand { get; }
    public DelegateCommand RestartMonitorCommand { get; }

    public ContainersControlViewModel(
        IMessageBoxService messageBoxService,
        IEventAggregator eventAggregator,
        ViewHelper viewHelper)
    {
        _messageBoxService = messageBoxService;
        _eventAggregator = eventAggregator;
        _viewHelper = viewHelper;
        _messages = new();

        ContainersCommand = new DelegateCommand(ContainersCommandExecute);
        ForceRemoveCommand = new DelegateCommand(ForceRemoveCommandExecute, () => SelectedContainer != null);
        PruneCommand = new DelegateCommand(PruneCommandExecute);
        CopyContainerEventsCommand = new DelegateCommand(CopyContainerEventsCommandExecute);
        RestartMonitorCommand = new DelegateCommand(RestartMonitorCommandExecute);

        MonitorContainersAsync().Await(handler: StartMonitorContainersHandler);
    }

    private async void ContainersCommandExecute()
    {
        try
        {
            ProgressVisibility = Visibility.Visible;
            await LoadContainersAsync();
        }
        catch (Exception ex)
        {
            _eventAggregator.GetEvent<ExceptionEvent>().Publish(ex);
        }
        finally
        {
            ProgressVisibility = Visibility.Hidden;
        }
    }

    private async void ForceRemoveCommandExecute()
    {
        try
        {
            ChangeOperationsPanelState(OperationState.Blocked);
            var id = SelectedContainer.Id;
            await DockerContainers.ForceRemoveAsync(id);
            _eventAggregator.GetEvent<ForceRemoveContainerEvent>().Publish(id);
        }
        catch (Exception ex)
        {
            _eventAggregator.GetEvent<ExceptionEvent>().Publish(ex);
        }
        finally
        {
            ChangeOperationsPanelState(OperationState.Unblocked);
        }
    }

    private async void PruneCommandExecute()
    {
        try
        {
            ChangeOperationsPanelState(OperationState.Blocked);
            var deleted = await DockerContainers.PruneAsync();
            _eventAggregator.GetEvent<ContainerPruneEvent>().Publish(deleted);
        }
        catch (Exception ex)
        {
            _eventAggregator.GetEvent<ExceptionEvent>().Publish(ex);
        }
        finally
        {
            ChangeOperationsPanelState(OperationState.Unblocked);
        }
    }

    private void CopyContainerEventsCommandExecute()
    {
        if (_messages.Count > 0)
        {
            var json = JsonConvert.SerializeObject(_messages, Formatting.Indented);
            Clipboard.SetText(json);
        }
    }

    private async void RestartMonitorCommandExecute()
    {
        var blocked = true;
        try
        {
            ChangeOperationsPanelState(OperationState.Blocked);
            await LoadContainersAsync();
            ChangeOperationsPanelState(OperationState.Unblocked);
            blocked = false;
            await MonitorContainersAsync(true);
        }
        catch (TimeoutException ex)
        {
            _eventAggregator.GetEvent<ExceptionEvent>().Publish(new ExceptionWithHint(DockerDesktop.Hints.Run, ex));

            if (_messageBoxService.Show(DockerDesktop.Questions.Start))
            {
                _eventAggregator.GetEvent<DockerDesktopStartEvent>().Publish();
            }
        }
        catch (Exception ex)
        {
            _eventAggregator.GetEvent<ExceptionEvent>().Publish(ex);
        }
        finally
        {
            if (blocked)
            {
                ChangeOperationsPanelState(OperationState.Unblocked);
            }
        }
    }

    private void ChangeOperationsPanelState(OperationState state)
    {
        switch (state)
        {
            case OperationState.Blocked:
                IsContainerOperationsEnabled = false; ProgressVisibility = Visibility.Visible;
                break;
            case OperationState.Unblocked:
                IsContainerOperationsEnabled = true; ProgressVisibility = Visibility.Hidden;
                break;
            default:
                throw new InvalidOperationException($"Invalid enum, value was: {(int)state}");
        }
    }

    private void StartMonitorContainersHandler(Exception ex)
    {
        var exceptionEvent = _eventAggregator.GetEvent<ExceptionEvent>();
        if (ex is TimeoutException)
        {
            exceptionEvent.Publish(new ExceptionWithHint(DockerDesktop.Hints.Run, ex));

            if (_messageBoxService.Show(DockerDesktop.Questions.Start, MessageBoxTitle))
            {
                _eventAggregator.GetEvent<DockerDesktopStartEvent>().Publish();
            }
        }
        else
        {
            exceptionEvent.Publish(ex);
        }
    }

    private async Task LoadContainersAsync()
    {
        Containers.Clear();
        var containers = await DockerContainers.List.Async();
        if (containers.Count > 0)
        {
            Containers.AddRange(containers.Select(x => new ContainerModel { Id = x.ID, Name = x.Names[0] }));
            _viewHelper.ScrollConsole(_application.MainWindow, Consts.ViewNames.ContainersConsole);
        }
        _eventAggregator.GetEvent<ListContainersEvent>().Publish(containers);
    }

    private async Task MonitorContainersAsync(bool command = false)
    {
        var progress = new Progress<Message>(async m =>
        {
            var containerKey = Consts.Keys.ContainerKey;
            var emptyKey = Consts.Keys.EmptyKey;
            var containerName = m.Actor.Attributes.TryGetValue("name", out var cont) && m.Type == containerKey ? cont : emptyKey;
            var imageName = m.Actor.Attributes.TryGetValue("image", out var img) && m.Type == containerKey ? img : emptyKey;

            _eventAggregator.GetEvent<ContainerMonitorEvent>().Publish((m.Action, m.Type, m.Actor.ID, containerName, imageName));
            _messages.Add(m);

            if (m.Type == containerKey && m.Action == "destroy" && Containers.FirstOrDefault(x => x.Id == m.Actor.ID) is ContainerModel container)
            {
                Containers.Remove(container);
            }
            else if (m.Type == containerKey && m.Action == "create" && containerName != emptyKey)
            {
                var initialized = Containers.Count > 0;
                if (initialized)
                {
                    Containers.Add(new ContainerModel { Id = m.Actor.ID, Name = containerName });
                }
                else
                {
                    await LoadContainersAsync();
                }
            }
        });

        if (command)
        {
            if (_monitorCts == null)
            {
                _monitorCts = new CancellationTokenSource();
            }
            else
            {
                _monitorCts.Cancel();
                _monitorCts = new CancellationTokenSource();
            }
            await DockerContainers.MonitorAsync(progress, new ContainerEventsParameters(), _monitorCts.Token);
        }
        else
        {
            await DockerContainers.MonitorAsync(progress);
        }
    }
}
