using DockerHelper.Core.Events;
using DockerHelper.Core.Extensions;
using DockerHelper.Core.Mvvm.ViewModels;
using DockerHelper.Core.Services;
using DockerHelper.Core.Utils;
using DockerHelper.Modules.Docker.Configurations;
using DockerHelper.Modules.Docker.Models;
using DockerHelper.Modules.Docker.Utils;
using Prism.Commands;
using Prism.Events;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace DockerHelper.Modules.Docker.ViewModels.Dialogs;

public class PortsDialogControlViewModel : DialogViewModel
{
    private readonly IEventAggregator _eventAggregator;
    private readonly ViewHelper _viewHelper;

    private string _externalPort = Consts.Keys.DefaultKey;
    public string ExternalPort
    {
        get => _externalPort;
        set
        {
            SetProperty(ref _externalPort, value);
            AddPortPairCommand.RaiseCanExecuteChanged();
        }
    }

    private string _internalPort = Consts.Keys.DefaultKey;
    public string InternalPort
    {
        get => _internalPort;
        set
        {
            SetProperty(ref _internalPort, value);
            AddPortPairCommand.RaiseCanExecuteChanged();
        }
    }

    private PortPairModel _selectedPortPair;
    public PortPairModel SelectedPortPair
    {
        get => _selectedPortPair;
        set
        {
            SetProperty(ref _selectedPortPair, value);
            RemovePortPairCommand.RaiseCanExecuteChanged();
        }
    }

    private ObservableCollection<PortPairModel> _portPairs;
    public ObservableCollection<PortPairModel> PortPairs
    {
        get => _portPairs ??= new();
        set => SetProperty(ref _portPairs, value);
    }

    private Visibility _toolTipVisibility = Visibility.Collapsed;
    public Visibility ToolTipVisibility
    {
        get => _toolTipVisibility;
        set => SetProperty(ref _toolTipVisibility, value);
    }

    public DelegateCommand AddPortPairCommand { get; }
    public DelegateCommand RemovePortPairCommand { get; }
    public DelegateCommand SaveCommand { get; }

    public PortsDialogControlViewModel(IEventAggregator eventAggregator, ViewHelper viewHelper)
    {
        _eventAggregator = eventAggregator;
        _viewHelper = viewHelper;

        AddPortPairCommand = new DelegateCommand(AddPortPairCommandExecute, () => !ExternalPort.IsEmpty() && !InternalPort.IsEmpty());
        RemovePortPairCommand = new DelegateCommand(RemovePortPairCommandExecute, () => SelectedPortPair != null);
        SaveCommand = new DelegateCommand(SaveCommandExecute);
    }

    public override void OnDialogOpened(IDialogParameters parameters)
    {
        base.OnDialogOpened(parameters);
        var ports = parameters.GetValue<List<PortPairConfiguration>>(Consts.Keys.PortsKey);
        foreach (var p in ports)
        {
            PortPairs.Add(new PortPairModel { ExternalPort = p.ExternalPort, InternalPort = p.InternalPort });
        }
    }

    private void AddPortPairCommandExecute()
    {
        try
        {
            var externalPort = ExternalPort == Consts.Keys.DefaultKey ? DockerConfig.GetExternalPort() : int.Parse(ExternalPort);
            var internalPort = InternalPort == Consts.Keys.DefaultKey ? DockerConfig.GetInternalPort() : int.Parse(InternalPort);

            PortPairs.Add(new PortPairModel { ExternalPort = externalPort, InternalPort = internalPort });
            _viewHelper.ScrollDialogConsole(Consts.ViewNames.PortsConsole);
            ExternalPort = InternalPort = Consts.Keys.DefaultKey;
            ToolTipVisibility = Visibility.Visible;
        }
        catch (Exception ex)
        {
            _eventAggregator.GetEvent<ExceptionEvent>().Publish(ex);
        }
    }

    private void RemovePortPairCommandExecute()
    {
        PortPairs.Remove(SelectedPortPair);
        SelectedPortPair = null;
        ToolTipVisibility = PortPairs.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void SaveCommandExecute()
    {
        var result = new DialogResult(ButtonResult.OK);
        result.Parameters.Add(
            Consts.Keys.PortsKey, PortPairs.Select(x => new PortPairConfiguration(x.ExternalPort, x.InternalPort)).ToArray()
        );
        OnRequestClose(result);
    }
}
