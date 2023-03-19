using Prism.Events;
using Prism.Mvvm;
using System.Windows;

namespace DockerHelper.Core.Mvvm.ViewModels;

public abstract class BindingViewModel : BindableBase
{
    protected readonly System.Windows.Application _application;
    protected readonly Dictionary<EventBase, SubscriptionToken> _events;

    public BindingViewModel()
    {
        _application = System.Windows.Application.Current;
        _events = new();
    }
}
