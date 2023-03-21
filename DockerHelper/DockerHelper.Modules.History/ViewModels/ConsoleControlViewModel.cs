using Docker.DotNet.Models;
using DockerHelper.Core.Events;
using DockerHelper.Core.Mvvm.ViewModels;
using DockerHelper.Core.Services;
using DockerHelper.Core.Utils;
using DockerHelper.Modules.History.Models;
using Prism.Commands;
using Prism.Events;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace DockerHelper.Modules.History.ViewModels;

public class ConsoleControlViewModel : ThreadSaveViewModel
{
    private readonly IEventAggregator _eventAggregator;
    private readonly ViewHelper _viewHelper;

    private EntryModel _selectedEntry;
    public EntryModel SelectedEntry
    {
        get => _selectedEntry;
        set
        {
            SetProperty(ref _selectedEntry, value);
            CopyCommand.RaiseCanExecuteChanged();
        }
    }

    private ObservableCollection<EntryModel> _messages;
    public ObservableCollection<EntryModel> Messages
    {
        get => _messages ??= new();
        set => SetProperty(ref _messages, value);
    }

    public DelegateCommand CopyCommand { get; }
    public DelegateCommand ClearCommand { get; }

    public ConsoleControlViewModel(IEventAggregator eventAggregator, ViewHelper viewHelper)
    {
        _eventAggregator = eventAggregator;
        _viewHelper = viewHelper;

        SubscribeToEvents();

        CopyCommand = new DelegateCommand(CopyCommandExecute, () => Messages.Any() && SelectedEntry != null);
        ClearCommand = new DelegateCommand(() => Messages.Clear());
    }

    private void CopyCommandExecute()
    {
        switch (SelectedEntry)
        {
            case NameModel nameModel:
                Clipboard.SetText(nameModel.GetName());
                break;
            case IdModel idModel:
                Clipboard.SetText(idModel.GetId());
                break;
            default:
                Clipboard.SetText(SelectedEntry.Message);
                break;
        }
    }

    private void SubscribeToEvents()
    {
        var listContainersEvent = _eventAggregator.GetEvent<ListContainersEvent>();
        var listContainersEventToken = listContainersEvent.Subscribe(response => {
            var systemEntry = new EntryModel {
                Message = "docker ps -a", Brush = Brushes.Orange
            };
            var messages = new List<EntryModel> { systemEntry };
            if (response.Count == 0)
            {
                messages.Add(new EntryModel { Message = "return no result" });
            }
            else
            {
                messages.AddRange(ConvertFrom(response));
            }

            AddMessages(messages);
        });

        var listImagesEvent = _eventAggregator.GetEvent<ListImagesEvent>();
        var listImagesEventToken = listImagesEvent.Subscribe(response => {
            var systemEntry = new EntryModel {
                Message = "docker images", Brush = Brushes.Orange
            };
            var messages = new List<EntryModel> { systemEntry };
            if (response.Count == 0)
            {
                messages.Add(new EntryModel { Message = "return no result" });
            }
            else
            {
                messages.AddRange(ConvertFrom(response));
            }

            AddMessages(messages);
        });

        var imageRemovedEvent = _eventAggregator.GetEvent<ImageRemovedEvent>();
        var imageRemovedEventToken = imageRemovedEvent.Subscribe(removed => {
            var name = removed.FirstOrDefault(item => item.Tag == Consts.Keys.DeletedImageKey).Value;
            var systemEntry = new EntryModel {
                Message = $"docker rmi {name}", Brush = Brushes.Orange
            };
            var messages = new List<EntryModel> { systemEntry };
            removed.ForEach(item => messages.Add(new EntryModel { Message = $"{item.Tag}: {item.Value}" }));

            AddMessages(messages);
        });

        var runContainerEvent = _eventAggregator.GetEvent<RunContainerEvent>();
        var runContainerEventToken = runContainerEvent.Subscribe(state => {
            var systemEntry = new EntryModel {
                Message = state.Cmd == Consts.Keys.NotExistingCmdKey ? "error occurred" : state.Cmd,
                Brush = state.Cmd == Consts.Keys.NotExistingCmdKey ? Brushes.Firebrick : Brushes.Orange
            };
            var messages = new List<EntryModel> { systemEntry };
            if (state.Created)
            {
                messages.Add(new EntryModel { Message = "Container created successfully!" });
            }
            if (state.Running)
            {
                messages.Add(new EntryModel { Message = "Container started successfully!" });
            }
            if (state.Id != Consts.Keys.NotExistingIdKey)
            {
                messages.Add(new IdModel(state.Id) { Message = $"ID: {state.Id}" });
            }

            AddMessages(messages);
        });

        var containerMonitorEvent = _eventAggregator.GetEvent<ContainerMonitorEvent>();
        var containerMonitorEventToken = containerMonitorEvent.Subscribe(x => {
            var systemEntry = new EntryModel {
                Message = "new container monitor event received", Brush = Brushes.Orange
            };
            var messages = new List<EntryModel> {
                systemEntry,
                new EntryModel { Message = $"Type: {x.Type}; Action: {x.Action}" }
            };
            if (x.Type == "container")
            {
                messages.Add(new EntryModel { Message = $"Image: {x.Image}" });
                messages.Add(new EntryModel { Message = $"Container: {x.Container}" });
                messages.Add(new EntryModel { Message = $"Container ID: {x.Id}" });
            }
            else if (x.Type == "network")
            {
                messages.Add(new EntryModel { Message = $"Actor ID: {x.Id}" });
            }

            AddMessages(messages);
        });

        var forceRemoveContainerEvent = _eventAggregator.GetEvent<ForceRemoveContainerEvent>();
        var forceRemoveContainerEventToken = forceRemoveContainerEvent.Subscribe(id => {
            var systemEntry = new EntryModel {
                Message = $"docker rm --force {id}", Brush = Brushes.Orange
            };

            AddMessages(new[] { systemEntry });
        });

        var containerPruneEvent = _eventAggregator.GetEvent<ContainerPruneEvent>();
        var containerPruneEventToken = containerPruneEvent.Subscribe(deleted => {
            var systemEntry = new EntryModel {
                Message = "docker container prune", Brush = Brushes.Orange
            };
            var messages = new List<EntryModel> { systemEntry };
            foreach (var item in deleted)
            {
                messages.Add(new EntryModel { Message = $"Deleted: {item}" });
            }

            AddMessages(messages);
        });

        _events.Add(listContainersEvent, listContainersEventToken);
        _events.Add(listImagesEvent, listImagesEventToken);
        _events.Add(imageRemovedEvent, imageRemovedEventToken);
        _events.Add(runContainerEvent, runContainerEventToken);
        _events.Add(containerMonitorEvent, containerMonitorEventToken);
        _events.Add(forceRemoveContainerEvent, forceRemoveContainerEventToken);
        _events.Add(containerPruneEvent, containerPruneEventToken);
    }

    private void AddMessages(IEnumerable<EntryModel> messages)
    {
        Messages.AddRange(messages);
        CopyCommand.RaiseCanExecuteChanged();
        _viewHelper.ScrollConsole(_application.MainWindow, Consts.ViewNames.HistoryConsole);
    }

    private static List<EntryModel> ConvertFrom(IList<ImagesListResponse> responses)
    {
        var messages = new List<EntryModel>();
        foreach (var response in responses)
        {
            messages.Add(new NameModel(response.RepoTags[0]) { Message = $"Name: {response.RepoTags[0]}" });
            messages.Add(new IdModel(response.ID) { Message = $"ID: {response.ID}" });
        }
        return messages;
    }

    private static List<EntryModel> ConvertFrom(IList<ContainerListResponse> responses)
    {
        var messages = new List<EntryModel>();
        foreach (var response in responses)
        {
            messages.Add(new NameModel(response.Names[0]) { Message = $"Name: {response.Names[0]}" });
            messages.Add(new IdModel(response.ID) { Message = $"ID: {response.ID}" });
        }
        return messages;
    }
}
