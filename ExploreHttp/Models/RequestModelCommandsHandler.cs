using ExploreHttp.Services;
using System.Windows;
using System.Windows.Input;

namespace ExploreHttp.Models;
public class RequestModelCommandsHandler
{
    private readonly Window _hostWindow;
    private readonly ApplicationViewModel _vm;
    private readonly Action<int> _selectTabByIndex;
    private readonly RequestRunner _runner;

    public RequestModelCommandsHandler(Window hostWindow, ApplicationViewModel vm, Action<int> selectTabByIndex)
    {
        _hostWindow = hostWindow;
        _vm = vm;
        _selectTabByIndex = selectTabByIndex;
        _runner = new RequestRunner(vm.AppSettings);
    }

    public void NewRequestCommandHandler(object sender, ExecutedRoutedEventArgs e)
    {
        var collection = e.Parameter as RequestCollection;
        var savedRequest = new SavedRequest(collection)
        {
            Name = "New Request",
            Method = RequestMethod.Get,
            Url = string.Empty
        };
        collection.SavedRequests.Add(savedRequest);
        collection.UnsavedChangesIndicatorVisibility = Visibility.Visible;
        var request = new RequestModel(savedRequest)
        {
            Id = savedRequest.Id,
            Name = savedRequest.Name,
            Method = savedRequest.Method,
            Url = savedRequest.Url,
            UnsavedChangesIndicatorVisibility = Visibility.Visible,
        };
        request.RequestHeaders.Headers.Add(new HeaderItemModel("User-Agent", _vm.AppSettings.UserAgentString));
        _vm.OpenRequests.Add(request);
        _selectTabByIndex(_vm.OpenRequests.Count - 1);
    }

    public void CloseRequestCommandHandler(object sender, ExecutedRoutedEventArgs e)
    {
        var request = e.Parameter as RequestModel;
        if (request.UnsavedChangesIndicatorVisibility == Visibility.Visible)
        {
            if (MessageBox.Show(_hostWindow,
                                "Are you sure you wish to close this unsaved request?",
                                "There are unsaved changes",
                                MessageBoxButton.OKCancel,
                                MessageBoxImage.Exclamation) == MessageBoxResult.Cancel)
            {
                return;
            }
        }

        _vm.OpenRequests.Remove(request);
    }

    public void OpenSavedRequestCommandHandler(object sender, ExecutedRoutedEventArgs e)
    {
        var savedRequest = e.Parameter as SavedRequest;
        var alreadyOpenRequest = _vm.OpenRequests.FirstOrDefault(x => x.Id == savedRequest.Id);
        if (alreadyOpenRequest is not null)
        {
            _selectTabByIndex(_vm.OpenRequests.IndexOf(alreadyOpenRequest));
            return;
        }

        var loadedRequestModel = savedRequest.ParentCollection.Loader?.LoadRequest(savedRequest.Id);
        if (loadedRequestModel is null)
        {
            var request = new RequestModel(savedRequest)
            {
                Id = savedRequest.Id,
                Name = savedRequest.Name,
                Method = savedRequest.Method,
                Url = savedRequest.Url,
                UnsavedChangesIndicatorVisibility = Visibility.Visible
            };
            request.RequestHeaders.Headers.Add(new HeaderItemModel("User-Agent", _vm.AppSettings.UserAgentString));
            _vm.OpenRequests.Add(request);
            _selectTabByIndex(_vm.OpenRequests.Count - 1);
            return;
        }

        var loadedRequest = ModelConverter.FromStorage(loadedRequestModel, savedRequest);
        loadedRequest.UnsavedChangesIndicatorVisibility = Visibility.Collapsed;
        _vm.OpenRequests.Add(loadedRequest);
        _selectTabByIndex(_vm.OpenRequests.Count - 1);
    }

    public void SaveRequestCommandHandler(object sender, ExecutedRoutedEventArgs e)
    {
        var request = e.Parameter as RequestModel;

        request.SavedRequest.Name = request.Name;
        request.SavedRequest.Url = request.Url;
        request.SavedRequest.Method = request.Method;
        request.SavedRequest.ParentCollection.UnsavedChangesIndicatorVisibility = Visibility.Collapsed;

        var requestToStore = ModelConverter.ToStorage(request);
        var metadata = ModelConverter.ToStorage(request.SavedRequest.ParentCollection);
        request.SavedRequest.ParentCollection.Loader.UpdateMetadata(metadata);
        request.SavedRequest.ParentCollection.Loader.SaveRequest(requestToStore);
        request.UnsavedChangesIndicatorVisibility = Visibility.Collapsed;
    }

    public void DeleteRequestCommandHandler(object sender, ExecutedRoutedEventArgs e)
    {
        var savedRequest = e.Parameter as SavedRequest;

        if (MessageBox.Show(_hostWindow,
            "Are you sure you want to remove this request from collection?",
            "Confirm reqest removal",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question) == MessageBoxResult.Yes)
        {
            savedRequest.ParentCollection.SavedRequests.Remove(savedRequest);
            savedRequest.ParentCollection.Loader.RemoveRequest(savedRequest.Id);
            var metadata = ModelConverter.ToStorage(savedRequest.ParentCollection);
            savedRequest.ParentCollection.Loader.UpdateMetadata(metadata);

            savedRequest.ParentCollection.UnsavedChangesIndicatorVisibility = Visibility.Collapsed;
        }
    }


    public async void RunRequestCommandHandler(object sender, ExecutedRoutedEventArgs e)
    {
        var requestModel = e.Parameter as RequestModel;
        await _runner.RunRequest(requestModel);
    }

    public void ViewLogsCommandHandler(object sender, ExecutedRoutedEventArgs e)
    {
        var requestModel = e.Parameter as RequestModel;
        LogWindow.OpenDialog(_hostWindow, requestModel);
    }

    public void BindAllCommands(CommandBindingCollection bindings)
    {
        bindings.Add(new CommandBinding(Application.Current.FindResource(CommandNames.NewRequestCommandName) as ICommand, NewRequestCommandHandler));
        bindings.Add(new CommandBinding(Application.Current.FindResource(CommandNames.CloseRequestCommandName) as ICommand, CloseRequestCommandHandler));
        bindings.Add(new CommandBinding(Application.Current.FindResource(CommandNames.OpenRequestFromCollectionCommandName) as ICommand, OpenSavedRequestCommandHandler));
        bindings.Add(new CommandBinding(Application.Current.FindResource(CommandNames.SaveRequestCommandName) as ICommand, SaveRequestCommandHandler));
        bindings.Add(new CommandBinding(Application.Current.FindResource(CommandNames.DeleteRequestCommandName) as ICommand, DeleteRequestCommandHandler));

        bindings.Add(new CommandBinding(Application.Current.FindResource(CommandNames.RunRequestCommandName) as ICommand, RunRequestCommandHandler));
        bindings.Add(new CommandBinding(Application.Current.FindResource(CommandNames.ViewRequestLogsCommandName) as ICommand, ViewLogsCommandHandler));
    }
}
