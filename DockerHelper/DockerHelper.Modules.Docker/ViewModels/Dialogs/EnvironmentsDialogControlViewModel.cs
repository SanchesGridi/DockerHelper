using DockerHelper.Core.Extensions;
using DockerHelper.Core.Mvvm.ViewModels;
using DockerHelper.Core.Services;
using DockerHelper.Core.Utils;
using DockerHelper.Modules.Docker.Configurations;
using DockerHelper.Modules.Docker.Models;
using DockerHelper.Modules.Docker.Utils;
using Prism.Commands;
using Prism.Services.Dialogs;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace DockerHelper.Modules.Docker.ViewModels.Dialogs;

public class EnvironmentsDialogControlViewModel : DialogViewModel
{
    private readonly ViewHelper _viewHelper;

    private string _selectedEnvKey;
    public string SelectedEnvKey
    {
        get => _selectedEnvKey;
        set
        {
            SetProperty(ref _selectedEnvKey, value);
            SetEnvKeyCommand.RaiseCanExecuteChanged();
        }
    }

    private ObservableCollection<string> _envKeys;
    public ObservableCollection<string> EnvKeys
    {
        get => _envKeys ??= new(EnvVars.AspList());
        set => SetProperty(ref _envKeys, value);
    }

    private string _currentKey;
    public string CurrentKey
    {
        get => _currentKey;
        set
        {
            SetProperty(ref _currentKey, value);
            AddEnvVariableCommand.RaiseCanExecuteChanged();
        }
    }

    private string _currentValue;
    public string CurrentValue
    {
        get => _currentValue;
        set
        {
            SetProperty(ref _currentValue, value);
            AddEnvVariableCommand.RaiseCanExecuteChanged();
        }
    }

    private EnvVarModel _selectedEnv;
    public EnvVarModel SelectedEnv
    {
        get => _selectedEnv;
        set
        {
            SetProperty(ref _selectedEnv, value);
            RemoveEnvVariableCommand.RaiseCanExecuteChanged();
        }
    }

    private ObservableCollection<EnvVarModel> _envs;
    public ObservableCollection<EnvVarModel> Envs
    {
        get => _envs ??= new();
        set => SetProperty(ref _envs, value);
    }

    private Visibility _toolTipVisibility = Visibility.Collapsed;
    public Visibility ToolTipVisibility
    {
        get => _toolTipVisibility;
        set => SetProperty(ref _toolTipVisibility, value);
    }

    public DelegateCommand SetEnvKeyCommand { get; }
    public DelegateCommand AddEnvVariableCommand { get; }
    public DelegateCommand RemoveEnvVariableCommand { get; }
    public DelegateCommand SaveCommand { get; }

    public EnvironmentsDialogControlViewModel(ViewHelper viewHelper)
    {
        _viewHelper = viewHelper;

        SetEnvKeyCommand = new DelegateCommand(SetEnvKeyCommandExecute, () => !SelectedEnvKey.IsEmpty());
        AddEnvVariableCommand = new DelegateCommand(AddEnvVariableCommandExecute, () => !CurrentKey.IsEmpty() && !CurrentValue.IsEmpty());
        RemoveEnvVariableCommand = new DelegateCommand(RemoveEnvVariableCommandExecute, () => SelectedEnv != null);
        SaveCommand = new DelegateCommand(SaveCommandExecute);
    }

    public override void OnDialogOpened(IDialogParameters parameters)
    {
        base.OnDialogOpened(parameters);
        var envs = parameters.GetValue<List<EnvVarConfiguration>>(Consts.Keys.EnvsKey);
        foreach (var e in envs)
        {
            Envs.Add(new EnvVarModel { Key = e.Key, Value = e.Value });
        }
    }

    private void SetEnvKeyCommandExecute() => CurrentKey = SelectedEnvKey;

    private void AddEnvVariableCommandExecute()
    {
        Envs.Add(new EnvVarModel { Key = CurrentKey, Value = CurrentValue });
        _viewHelper.ScrollDialogConsole(Consts.ViewNames.EnvsConsole);
        CurrentKey = CurrentValue = string.Empty;
        ToolTipVisibility = Visibility.Visible;
    }

    private void RemoveEnvVariableCommandExecute()
    {
        Envs.Remove(SelectedEnv);
        SelectedEnv = null;
        ToolTipVisibility = Envs.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void SaveCommandExecute()
    {
        var result = new DialogResult(ButtonResult.OK);
        result.Parameters.Add(
            Consts.Keys.EnvsKey, Envs.Select(x => new EnvVarConfiguration(x.Key, x.Value)).ToArray()
        );
        OnRequestClose(result);
    }
}
