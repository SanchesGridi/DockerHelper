using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using DockerHelper.Core.Utils; 

namespace DockerHelper.Core.Mvvm.ViewModels;

public abstract class DialogViewModel : BindableBase, IDialogAware
{
    private string _title;
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public DelegateCommand CloseCommand { get; }

    public event Action<IDialogResult> RequestClose;

    public DialogViewModel()
    {
        CloseCommand = new DelegateCommand(CloseCommandExecute);
    }

    public virtual bool CanCloseDialog() => true;

    public virtual void OnDialogOpened(IDialogParameters parameters) =>
        Title = parameters.GetValue<string>(Consts.Keys.TitleKey);

    protected void OnRequestClose(IDialogResult result) =>
        RequestClose?.Invoke(result);

    protected virtual void CloseCommandExecute() =>
        OnRequestClose(new Prism.Services.Dialogs.DialogResult(ButtonResult.None));

    public virtual void OnDialogClosed()
    {
    }
}
