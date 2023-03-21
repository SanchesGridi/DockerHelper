using DockerHelper.Core.Events;
using DockerHelper.Core.Exceptions;
using DockerHelper.Core.Mvvm.ViewModels;
using DockerHelper.Core.Services;
using DockerHelper.Core.Utils;
using DockerHelper.Modules.Exceptions.Models;
using Prism.Commands;
using Prism.Events;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace DockerHelper.Modules.Exceptions.ViewModels;

public class ConsoleControlViewModel : ThreadSaveViewModel
{
    private readonly IEventAggregator _eventAggregator;
    private readonly ViewHelper _viewHelper;

    private ObservableCollection<EntryModel> _exceptions;
    public ObservableCollection<EntryModel> Exceptions
    {
        get => _exceptions ??= new();
        set => SetProperty(ref _exceptions, value);
    }

    public DelegateCommand ClearConsoleCommand { get; }

    public ConsoleControlViewModel(IEventAggregator eventAggregator, ViewHelper viewHelper)
    {
        _eventAggregator = eventAggregator;
        _viewHelper = viewHelper;

        var exceptionEvent = _eventAggregator.GetEvent<ExceptionEvent>();
        var ExceptionEventToken = exceptionEvent.Subscribe(async (ex) => {
            var entry = new EntryModel();
            entry.SetException(ex);
            if (ex.GetType() == typeof(ExceptionWithHint))
            {
                entry.Brush = Brushes.LightGreen;
            }
            await DispatchAsync(() =>
            {
                Exceptions.AddRange(entry.GetInners());
                _viewHelper.ScrollConsole(_application.MainWindow, Consts.ViewNames.ExceptionsConsole);
            });
        });

        _events.Add(exceptionEvent, ExceptionEventToken);

        ClearConsoleCommand = new DelegateCommand(() => Exceptions.Clear());
    }
}
