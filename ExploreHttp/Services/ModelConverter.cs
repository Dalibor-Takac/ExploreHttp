using ExploreHttp.Models;
using ExploreHttp.Services.PersistanceModels;
using System.Collections.ObjectModel;

namespace ExploreHttp.Services;
public class ModelConverter
{
    public static RequestCollection FromStorage(EndpointCollection collection)
    {
        var result = new RequestCollection()
        {
            CollectionName = collection.Name,
            IsExpanded = false,
            Kind = collection.Kind,
            Source = collection.Source,
            UnsavedChangesIndicatorVisibility = System.Windows.Visibility.Collapsed,
            SavedRequests = new ObservableCollection<SavedRequest>(collection.Requests.Select(x => new SavedRequest()
            {
                Id = x.Id,
                Method = x.Method,
                Name = x.Name,
                Url = x.Url
            })),
            SavedEnvironments = new ObservableCollection<SavedEnvironment>(collection.Environments.Select(x => new SavedEnvironment()
            {
                Id = x.Id,
                Name = x.Name,
                Variables = new ObservableCollection<EnvironmentVariable>(x.Variables.Select(v => new EnvironmentVariable()
                {
                    Name = v.Name,
                    Value = v.Value,
                    IsEnabled = v.IsEnabled
                }))
            })),
            SelectedEnvironmentIndex = collection.SelectedEnvironmentIndex
        };

        return result;
    }

    public static EndpointCollection ToStorage(RequestCollection collection)
    {
        var result = new EndpointCollection()
        {
            Name = collection.CollectionName,
            Kind = collection.Kind,
            Source = collection.Source,
            Requests = collection.SavedRequests.Select(x => new RequestInfo()
            {
                Id = x.Id,
                Method = x.Method,
                Name = x.Name,
                Url = x.Url
            }).ToList(),
            Environments = collection.SavedEnvironments.Select(x => new CollectionEnvironment()
            {
                Id = x.Id,
                Name = x.Name,
                Variables = x.Variables.Select(v => new Variable() { IsEnabled = v.IsEnabled, Name = v.Name, Value = v.Value }).ToList()
            }).ToList(),
            SelectedEnvironmentIndex = collection.SelectedEnvironmentIndex
        };

        return result;
    }

    public static RequestModel FromStorage(Request request)
    {
        var result = new RequestModel()
        {
            Id = request.Id,
            Name = request.Name,
            Method = request.Method,
            Url = request.Url,
            UnsavedChangesIndicatorVisibility = System.Windows.Visibility.Collapsed,
            RequestHeaders = new HeaderCollection()
            {
                Headers = new ObservableCollection<HeaderItemModel>(request.RequestHeaders.Select(x => new HeaderItemModel()
                {
                    IsEnabled = x.IsEnabled,
                    HeaderName = x.HeaderName,
                    HeaderValue = x.HeaderValue
                })),
                IsEditable = true
            },
            RequestBody = new BodyProvider()
            {
                Type = request.RequestBody.Kind,
                Source = request.RequestBody.Source,
            },
            ResponseStatus = request.ResponseStatus,
            ResponseDuration = request.ResponseDuration,
            ResponseHeaders = new HeaderCollection()
            {
                Headers = new ObservableCollection<HeaderItemModel>(request.ResponseHeaders.Select(x => new HeaderItemModel()
                {
                    IsEnabled = x.IsEnabled,
                    HeaderName = x.HeaderName,
                    HeaderValue = x.HeaderValue
                })),
                IsEditable = false,
            },
            ResponseBody = new BodyProvider()
            {
                Type = request.ResponseBody.Kind,
                Source = request.ResponseBody.Source
            },
            Logs = new ObservableCollection<LogRecord>(request.Logs.Select(x => new LogRecord()
            {
                Level = x.Level,
                Timestamp = x.Timestamp,
                Message = x.Message,
                Properties = new ObservableCollection<LogProperty>(x.Properties.Select(p => new LogProperty()
                {
                    PropertyName = p.Key,
                    PropertyValue = p.Value
                }))
            }))
        };

        return result;
    }

    public static Request ToStorage(RequestModel request)
    {
        var result = new Request()
        {
            Id = request.Id,
            Name = request.Name,
            Method = request.Method,
            Url = request.Url,
            RequestHeaders = request.RequestHeaders.Headers.Select(x => new HeaderItem()
            {
                HeaderName = x.HeaderName,
                HeaderValue = x.HeaderValue,
                IsEnabled = x.IsEnabled
            }).ToList(),
            RequestBody = new ContentProvider()
            {
                Kind = request.RequestBody.Type,
                Source = request.RequestBody.Source
            },
            ResponseStatus = request.ResponseStatus,
            ResponseDuration = request.ResponseDuration,
            ResponseSize = request.ResponseSize,
            ResponseHeaders = request.ResponseHeaders.Headers.Select(x => new HeaderItem()
            {
                HeaderName = x.HeaderName,
                HeaderValue = x.HeaderValue,
                IsEnabled = x.IsEnabled
            }).ToList(),
            ResponseBody = new ContentProvider()
            {
                Kind = request.ResponseBody.Type,
                Source = request.ResponseBody.Source
            },
            Logs = request.Logs.Select(x => new LogEvent()
            {
                Level = x.Level,
                Timestamp = x.Timestamp,
                Message = x.Message,
                Properties = x.Properties.ToDictionary(p => p.PropertyName, p => p.PropertyValue)
            }).ToList()
        };

        return result;
    }
}
