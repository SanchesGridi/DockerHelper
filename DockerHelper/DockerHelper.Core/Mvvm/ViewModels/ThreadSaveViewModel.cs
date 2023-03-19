namespace DockerHelper.Core.Mvvm.ViewModels
{
    public abstract class ThreadSaveViewModel : BindingViewModel
    {
        protected virtual async Task DispatchAsync(Action action)
        {
            await _application.Dispatcher.InvokeAsync(action);
        }
    }
}
