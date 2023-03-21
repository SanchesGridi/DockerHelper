using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace DockerHelper.Modules.Exceptions.Models;

public class EntryModel : BindableBase
{
    private string _message;
    public string Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }

    private Exception _exception;
    public Exception Exception
    {
        get => _exception;
        set => SetProperty(ref _exception, value);
    }

    private Brush _brush;
    public Brush Brush
    {
        get => _brush ??= Brushes.Firebrick;
        set => SetProperty(ref _brush, value);
    }

    public void SetException(Exception exception)
    {
        Exception = exception;
        Message = Exception.Message;
    }

    public List<EntryModel> GetInners()
    {
        var store = new List<EntryModel> { this };
        if (Exception.InnerException != null)
        {
            var entry = new EntryModel();
            entry.SetException(Exception.InnerException);
            store.AddRange(entry.GetInners());
        }
        return store;
    }
}
