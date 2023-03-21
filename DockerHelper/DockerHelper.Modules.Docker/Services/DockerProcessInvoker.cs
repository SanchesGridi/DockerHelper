using DockerHelper.Core.Events;
using DockerHelper.Core.Exceptions;
using DockerHelper.Modules.Docker.Utils;
using Prism.Events;
using System;
using System.Diagnostics;

namespace DockerHelper.Modules.Docker.Services;

public class DockerProcessInvoker : IProcessInvoker
{
    private const string DockerFileName = "C:\\Program Files\\Docker\\Docker\\Docker Desktop.exe";
    private const string DockerProcessName = "Docker Desktop";

    private readonly IEventAggregator _eventAggregator;

    private Process _process;

    public DockerProcessInvoker(IEventAggregator eventAggregator)
    {
        _eventAggregator = eventAggregator;
    }

    public void Invoke()
    {
        try
        {
            var dockerProcesses = Process.GetProcessesByName(DockerProcessName);
            if (dockerProcesses != null && dockerProcesses.Length > 0)
            {
                throw new ExceptionWithHint(DockerDesktop.Hints.AlreadyStarted);
            }
            else
            {
                if (_process == null)
                {
                    Start();
                }
                else
                {
                    Stop();
                    Start();
                }
            }
        }
        catch (Exception ex)
        {
            _eventAggregator.GetEvent<ExceptionEvent>().Publish(ex);
        }
    }

    private void Start()
    {
        _process = Process.Start(DockerFileName);
    }

    private void Stop()
    {
        _process.Kill();
        _process.WaitForExit();
        _process.Dispose();
    }
}
