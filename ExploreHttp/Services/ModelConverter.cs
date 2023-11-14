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
            IsExpanded = collection.IsExpanded,
            Kind = collection.Kind,
            Source = collection.Source,
            UnsavedChangesIndicatorVisibility = System.Windows.Visibility.Collapsed,
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
            SelectedEnvironmentIndex = collection.SelectedEnvironmentIndex,
            AuthProvider = new AuthenticationProvider()
            {
                Kind = collection.AuthKind,
                Basic = new BasicAuthenticationModel() { Username = collection.Basic?.Username, Password = collection.Basic?.Password },
                Bearer = new BearerAuthenticationModel() { Scheme = collection.Bearer?.Scheme, Parameter = collection.Bearer?.Parameter },
                Oauth2 = new Oauth2AuthenticationModel()
                {
                    AuthUrl = collection.Oauth2?.AuthUrl,
                    GrantType = collection.Oauth2?.GrantType ?? Oauth2GrantType.ClientCredentials,
                    ClientId = collection.Oauth2?.ClientId,
                    ClientSecret = collection.Oauth2?.ClientSecret,
                    Username = collection.Oauth2?.Username,
                    Password = collection.Oauth2?.Password,
                    Scope = collection.Oauth2?.Scope,
                    Audience = collection.Oauth2?.Audience
                }
            }
        };

        result.SavedRequests = new ObservableCollection<SavedRequest>(collection.Requests.Select(x => new SavedRequest(result)
        {
            Id = x.Id,
            Method = x.Method,
            Name = x.Name,
            Url = x.Url,
            OperationId = x.OperationId
        }));

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
                Url = x.Url,
                OperationId = x.OperationId
            }).ToList(),
            Environments = collection.SavedEnvironments.Select(x => new CollectionEnvironment()
            {
                Id = x.Id,
                Name = x.Name,
                Variables = x.Variables.Select(v => new Variable() { IsEnabled = v.IsEnabled, Name = v.Name, Value = v.Value }).ToList()
            }).ToList(),
            SelectedEnvironmentIndex = collection.SelectedEnvironmentIndex,
            IsExpanded = collection.IsExpanded,
            AuthKind = collection.AuthProvider.Kind,
            Basic = new BasicAuth() { Username = collection.AuthProvider.Basic.Username, Password = collection.AuthProvider.Basic.Password },
            Bearer = new BearerAuth() { Scheme = collection.AuthProvider.Bearer.Scheme, Parameter = collection.AuthProvider.Bearer.Parameter },
            Oauth2 = new Oauth2Auth()
            {
                AuthUrl = collection.AuthProvider.Oauth2.AuthUrl,
                GrantType = collection.AuthProvider.Oauth2.GrantType,
                ClientId = collection.AuthProvider.Oauth2.ClientId,
                ClientSecret = collection.AuthProvider.Oauth2.ClientSecret,
                Username = collection.AuthProvider.Oauth2.Username,
                Password = collection.AuthProvider.Oauth2.Password,
                Scope = collection.AuthProvider.Oauth2.Scope,
                Audience = collection.AuthProvider.Oauth2.Audience
            }
        };

        return result;
    }

    public static RequestModel FromStorage(Request request, SavedRequest savedRequest)
    {
        var result = new RequestModel(savedRequest)
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
                    HeaderName = x.Key,
                    HeaderValue = x.Value
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
            ResponseSize = request.ResponseSize,
            ResponseHeaders = new HeaderCollection()
            {
                Headers = new ObservableCollection<HeaderItemModel>(request.ResponseHeaders.Select(x => new HeaderItemModel()
                {
                    IsEnabled = x.IsEnabled,
                    HeaderName = x.Key,
                    HeaderValue = x.Value
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
            })),
            AuthProvider = new AuthenticationProvider()
            {
                Kind = request.AuthKind,
                Basic = new BasicAuthenticationModel() { Username = request.Basic?.Username, Password = request.Basic?.Password },
                Bearer = new BearerAuthenticationModel() { Scheme = request.Bearer?.Scheme, Parameter = request.Bearer?.Parameter },
                Oauth2 = new Oauth2AuthenticationModel()
                {
                    AuthUrl = request.Oauth2?.AuthUrl,
                    GrantType = request.Oauth2?.GrantType ?? Oauth2GrantType.ClientCredentials,
                    ClientId = request.Oauth2?.ClientId,
                    ClientSecret = request.Oauth2?.ClientSecret,
                    Username = request.Oauth2?.Username,
                    Password = request.Oauth2?.Password,
                    Scope = request.Oauth2?.Scope,
                    Audience = request.Oauth2?.Audience
                }
            }
        };

        result.QueryString = new QueryStringModel(result, request.QueryString?.Select(x => new QueryStringParameter()
        {
            IsEnabled = x.IsEnabled,
            ParameterName = x.Key,
            ParameterValue = x.Value
        }));
        result.QueryString.PropertyChanged += (sender, args) => { result.UnsavedChangesIndicatorVisibility = System.Windows.Visibility.Visible; };

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
            QueryString = request.QueryString.Parameters.Select(x => new KeyValuePairWithEnable()
            {
                IsEnabled = x.IsEnabled,
                Key = x.ParameterName,
                Value = x.ParameterValue
            }).ToList(),
            RequestHeaders = request.RequestHeaders.Headers.Select(x => new KeyValuePairWithEnable()
            {
                Key = x.HeaderName,
                Value = x.HeaderValue,
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
            ResponseHeaders = request.ResponseHeaders.Headers.Select(x => new KeyValuePairWithEnable()
            {
                Key = x.HeaderName,
                Value = x.HeaderValue,
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
            }).ToList(),
            AuthKind = request.AuthProvider.Kind,
            Basic = new BasicAuth() { Username = request.AuthProvider.Basic.Username, Password = request.AuthProvider.Basic.Password },
            Bearer = new BearerAuth() { Scheme = request.AuthProvider.Bearer.Scheme, Parameter = request.AuthProvider.Bearer.Parameter },
            Oauth2 = new Oauth2Auth()
            {
                AuthUrl = request.AuthProvider.Oauth2.AuthUrl,
                GrantType = request.AuthProvider.Oauth2.GrantType,
                ClientId = request.AuthProvider.Oauth2.ClientId,
                ClientSecret = request.AuthProvider.Oauth2.ClientSecret,
                Username = request.AuthProvider.Oauth2.Username,
                Password = request.AuthProvider.Oauth2.Password,
                Scope = request.AuthProvider.Oauth2.Scope,
                Audience = request.AuthProvider.Oauth2.Audience
            }
        };

        return result;
    }
}
