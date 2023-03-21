using DockerHelper.Core.Extensions;
using DockerHelper.Core.Mvvm.ViewModels;
using DockerHelper.Core.Services;
using DockerHelper.Core.Services.Interfaces;
using DockerHelper.Core.Utils;
using DockerHelper.Modules.Docker.Configurations;
using DockerHelper.Modules.Docker.Models;
using Prism.Commands;
using Prism.Services.Dialogs;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace DockerHelper.Modules.Docker.ViewModels.Dialogs;

public class VolumesDialogControlViewModel : DialogViewModel
{
    private readonly IFolderService _folderService;
    private readonly ViewHelper _viewHelper;

    private string _currentHostPath;
    public string CurrentHostPath
    {
        get => _currentHostPath;
        set
        {
            SetProperty(ref _currentHostPath, value);
            AddVolumeCommand.RaiseCanExecuteChanged();
        }
    }

    private string _currentContainerPath;
    public string CurrentContainerPath
    {
        get => _currentContainerPath;
        set
        {
            SetProperty(ref _currentContainerPath, value);
            AddVolumeCommand.RaiseCanExecuteChanged();
        }
    }

    private VolumeModel _selectedVolume;
    public VolumeModel SelectedVolume
    {
        get => _selectedVolume;
        set
        {
            SetProperty(ref _selectedVolume, value);
            RemoveVolumeCommand.RaiseCanExecuteChanged();
        }
    }

    private ObservableCollection<VolumeModel> _volumes;
    public ObservableCollection<VolumeModel> Volumes
    {
        get => _volumes ??= new();
        set => SetProperty(ref _volumes, value);
    }

    private Visibility _toolTipVisibility = Visibility.Collapsed;
    public Visibility ToolTipVisibility
    {
        get => _toolTipVisibility;
        set => SetProperty(ref _toolTipVisibility, value);
    }

    public DelegateCommand BrowseCommand { get; }
    public DelegateCommand AddVolumeCommand { get; }
    public DelegateCommand RemoveVolumeCommand { get; }
    public DelegateCommand SaveCommand { get; }

    public VolumesDialogControlViewModel(IFolderService folderService, ViewHelper viewHelper)
    {
        _folderService = folderService;
        _viewHelper = viewHelper;

        BrowseCommand = new DelegateCommand(BrowseCommandExecute);
        AddVolumeCommand = new DelegateCommand(AddVolumeCommandExecute, () => !CurrentHostPath.IsEmpty() && !CurrentContainerPath.IsEmpty());
        RemoveVolumeCommand = new DelegateCommand(RemoveVolumeCommandExecute, () => SelectedVolume != null);
        SaveCommand = new DelegateCommand(SaveCommandExecute);
    }

    public override void OnDialogOpened(IDialogParameters parameters)
    {
        base.OnDialogOpened(parameters);
        var volumes = parameters.GetValue<List<VolumeConfiguration>>(Consts.Keys.VolumesKey);
        foreach (var v in volumes)
        {
            Volumes.Add(new VolumeModel { HostPath = v.HostPath, ContainerPath = v.ContainerPath });
        }
    }

    private void BrowseCommandExecute() => CurrentHostPath = _folderService.Browse();

    private void AddVolumeCommandExecute()
    {
        Volumes.Add(new VolumeModel { HostPath = CurrentHostPath, ContainerPath = CurrentContainerPath });
        _viewHelper.ScrollDialogConsole(Consts.ViewNames.VolumesConsole);
        CurrentHostPath = CurrentContainerPath = string.Empty;
        ToolTipVisibility = Visibility.Visible;
    }

    private void RemoveVolumeCommandExecute()
    {
        Volumes.Remove(SelectedVolume);
        SelectedVolume = null;
        ToolTipVisibility = Volumes.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void SaveCommandExecute()
    {
        var result = new DialogResult(ButtonResult.OK);
        result.Parameters.Add(
            Consts.Keys.VolumesKey, Volumes.Select(x => new VolumeConfiguration(x.HostPath, x.ContainerPath)).ToArray()
        );
        OnRequestClose(result);
    }
}
