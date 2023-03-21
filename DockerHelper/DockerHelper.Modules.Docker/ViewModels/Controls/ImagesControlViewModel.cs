using Docker.DotNet;
using DockerHelper.Core.Events;
using DockerHelper.Core.Exceptions;
using DockerHelper.Core.Extensions;
using DockerHelper.Core.Mvvm.ViewModels;
using DockerHelper.Core.Services;
using DockerHelper.Core.Services.Interfaces;
using DockerHelper.Core.Utils;
using DockerHelper.Modules.Docker.Enums;
using DockerHelper.Modules.Docker.Models;
using DockerHelper.Modules.Docker.Utils;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DockerHelper.Modules.Docker.ViewModels.Controls;

public class ImagesControlViewModel : ThreadSaveViewModel
{
    private readonly IEventAggregator _eventAggregator;
    private readonly IMessageBoxService _messageBoxService;
    private readonly ViewHelper _viewHelper;

    private Visibility _progressVisibility = Visibility.Hidden;
    public Visibility ProgressVisibility
    {
        get => _progressVisibility;
        set => SetProperty(ref _progressVisibility, value);
    }

    private bool _isImageOperationsEnabled = true;
    public bool IsImageOperationsEnabled
    {
        get => _isImageOperationsEnabled;
        set => SetProperty(ref _isImageOperationsEnabled, value);
    }

    private ImageModel _selectedImage;
    public ImageModel SelectedImage
    {
        get => _selectedImage;
        set
        {
            SetProperty(ref _selectedImage, value);
            RemoveImageCommand.RaiseCanExecuteChanged();
            RunImageCommand.RaiseCanExecuteChanged();
            _eventAggregator.GetEvent<ImageNameChangedEvent>()
                .Publish(value == null ? string.Empty : GetImageIdOrName(value));
        }
    }

    private ObservableCollection<ImageModel> _images;
    public ObservableCollection<ImageModel> Images
    {
        get => _images ??= new();
        set => SetProperty(ref _images, value);
    }

    public DelegateCommand ImagesCommand { get; }
    public DelegateCommand RemoveImageCommand { get; }
    public DelegateCommand RunImageCommand { get; }

    public ImagesControlViewModel(
        IEventAggregator eventAggregator,
        IMessageBoxService messageBoxService,
        ViewHelper viewHelper)
    {
        _eventAggregator = eventAggregator;
        _messageBoxService = messageBoxService;
        _viewHelper = viewHelper;

        var someEvent = _eventAggregator.GetEvent<RunCompletedEvent>();
        var someToken = someEvent.Subscribe(() => ChangeOperationsPanelState(OperationState.Unblocked));

        _events.Add(someEvent, someToken);

        ImagesCommand = new DelegateCommand(async () => await LoadImagesAsync());
        RemoveImageCommand = new DelegateCommand(async () => await RemoveImageAsync(), () => Images.Any() && SelectedImage != null);
        RunImageCommand = new DelegateCommand(RunImageCommandExecute, () => Images.Any() && SelectedImage != null);
    }

    private void RunImageCommandExecute()
    {
        var performRun = false;
        try
        {
            ChangeOperationsPanelState(OperationState.Blocked);

            _eventAggregator.GetEvent<PerformRunEvent>().Publish(GetImageIdOrName(SelectedImage));
            performRun = true;
        }
        catch (Exception ex)
        {
            _eventAggregator.GetEvent<ExceptionEvent>().Publish(ex);
        }
        finally
        {
            if (!performRun)
            {
                ChangeOperationsPanelState(OperationState.Unblocked);
            }
        }
    }

    private async Task LoadImagesAsync()
    {
        var daemonException = false;
        try
        {
            ProgressVisibility = Visibility.Visible;

            Images.Clear();
            var images = await DockerImages.List.Async();
            _eventAggregator.GetEvent<ListImagesEvent>().Publish(images);
            await DispatchAsync(() => AddImagesAndScroll(images.Select(x => new ImageModel(x.ID, x.Size) { Name = x.RepoTags[0] })));
            RemoveImageCommand.RaiseCanExecuteChanged();
            RunImageCommand.RaiseCanExecuteChanged();
        }
        catch (TimeoutException ex)
        {
            _eventAggregator.GetEvent<ExceptionEvent>().Publish(new ExceptionWithHint(DockerDesktopHints.Run, ex));
            daemonException = true;
        }
        catch (DockerApiException ex) when (ex.Message == DockerConfig.EngineException)
        {
            _eventAggregator.GetEvent<ExceptionEvent>().Publish(new ExceptionWithHint(DockerDesktopHints.Troubleshoot, ex));
            daemonException = true;
        }
        catch (Exception ex)
        {
            _eventAggregator.GetEvent<ExceptionEvent>().Publish(ex);
        }
        finally
        {
            ProgressVisibility = Visibility.Hidden;

            if (daemonException && _messageBoxService.Show("Do you want to open the (Docker Desktop) application?"))
            {
                _eventAggregator.GetEvent<DockerDesktopStartEvent>().Publish();
            }
        }
    }

    private async Task RemoveImageAsync()
    {
        try
        {
            ChangeOperationsPanelState(OperationState.Blocked);

            var result = await DockerImages.Delete.Async(GetImageIdOrName(SelectedImage));
            _eventAggregator.GetEvent<ImageRemovedEvent>().Publish(result);
            Images.Remove(SelectedImage);
            SelectedImage = null;
        }
        catch (Exception ex)
        {
            _eventAggregator.GetEvent<ExceptionEvent>().Publish(ex);
        }
        finally
        {
            ChangeOperationsPanelState(OperationState.Unblocked);
        }
    }

    private void ChangeOperationsPanelState(OperationState state)
    {
        switch (state)
        {
            case OperationState.Blocked:
                IsImageOperationsEnabled = false; ProgressVisibility = Visibility.Visible;
                break;
            case OperationState.Unblocked:
                IsImageOperationsEnabled = true; ProgressVisibility = Visibility.Hidden;
                break;
            default:
                throw new InvalidOperationException($"Invalid enum, value was: {(int)state}");
        }
    }

    private void AddImagesAndScroll(IEnumerable<ImageModel> images)
    {
        Images.AddRange(images);
        _viewHelper.ScrollViewerScrollToEnd(_application.MainWindow, Consts.ViewNames.ImagesViewer);
    }

    private static string GetImageIdOrName(ImageModel image)
    {
        return image.Name == DockerConfig.UnnamedImage ? image.GetId() : image.Name;
    }
}
