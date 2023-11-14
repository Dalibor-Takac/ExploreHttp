using DotLiquid;
using ExploreHttp.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ExploreHttp.Services;
public class RequestRunner : IDisposable
{
    private readonly ObjectPool<HttpClient> _clientPool;
    private readonly AppSettings _settings;
    private readonly TokenHelper _tokenHelper;

    public RequestRunner(AppSettings settings)
    {
        _settings = settings;
        _clientPool = new ObjectPool<HttpClient>(
            settings.RequestPoolSize,
            () => new HttpClient(new LoggingHttpMessageHandler(_settings) { InnerHandler = new HttpClientHandler() }),
            client => client.CancelPendingRequests());
        _tokenHelper = new TokenHelper(_clientPool);
    }

    public static Hash EnvironmentToLocals(RequestCollection collection)
    {
        var result = new Hash();

        if (collection.SavedEnvironments.Count > collection.SelectedEnvironmentIndex)
        {
            var environment = collection.SavedEnvironments[collection.SelectedEnvironmentIndex];

            foreach (var item in environment.Variables)
            {
                result.Add(item.Name, item.Value);
            }
        }

        return result;
    }

    private async Task<HttpRequestMessage> ConstructRequest(RequestModel requestModel)
    {
        var result = new HttpRequestMessage();
        result.Method = requestModel.Method switch
        {
            RequestMethod.Get => HttpMethod.Get,
            RequestMethod.Post => HttpMethod.Post,
            RequestMethod.Put => HttpMethod.Put,
            RequestMethod.Patch => new HttpMethod("PATCH"),
            RequestMethod.Delete => HttpMethod.Delete,
            RequestMethod.Options => HttpMethod.Options,
            RequestMethod.Head => HttpMethod.Head,
            _ => throw new InvalidOperationException($"Unable to convert value {requestModel.Method} into HttpMethod")
        };

        var locals = EnvironmentToLocals(requestModel.SavedRequest.ParentCollection);

        var uriTemplate = Template.Parse(requestModel.Url);
        var uriBuilder = new UriBuilder(uriTemplate.Render(locals));
        if (requestModel.QueryString.Parameters.Count(x => x.IsEnabled) > 0)
            uriBuilder.Query = RenderQueryString(requestModel.QueryString, locals);
        result.RequestUri = uriBuilder.Uri;

        if (requestModel.AuthProvider != null && requestModel.AuthProvider.Kind != AuthenticationKind.None)
        {
            await AddAuthentication(requestModel, result);
        }
        
        if (requestModel.RequestBody is not null && !string.IsNullOrEmpty(requestModel.RequestBody.Source))
        {
            if (requestModel.RequestBody.Type == BodyType.Text)
            {
                var bodyTemplate = Template.Parse(requestModel.RequestBody.Source);
                var requestStream = new MemoryStream(Encoding.UTF8.GetBytes(bodyTemplate.Render(locals)));
                result.Content = new StreamContent(requestStream);
            }
            else
            {
                result.Content = new StreamContent(new FileStream(requestModel.RequestBody.Source, FileMode.Open, FileAccess.Read));
            }
        }

        var renderedHeaders = requestModel.RequestHeaders.Headers.Where(x => x.IsEnabled)
            .Select(x =>
            {
                var headerNameTemplate = Template.Parse(x.HeaderName);
                var headerValueTemplate = Template.Parse(x.HeaderValue);
                return new KeyValuePair<string, string>(headerNameTemplate.Render(locals), headerValueTemplate.Render(locals));
            })
            .ToArray();

        foreach (var header in renderedHeaders)
        {
            if (!result.Headers.TryAddWithoutValidation(header.Key, header.Value) && result.Content is not null)
            {
                result.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        result.Properties.Add(nameof(RequestModel), requestModel);

        return result;
    }

    private string RenderQueryString(QueryStringModel queryString, Hash locals)
    {
        var sb = new StringBuilder();
        var isFirst = true;
        foreach (var item in queryString.Parameters.Where(x => x.IsEnabled))
        {
            if (isFirst)
                isFirst = false;
            else
                sb.Append("&");
            var nameTemplate = Template.Parse(item.ParameterName);
            var valueTemplate = Template.Parse(item.ParameterValue);
            sb.Append($"{Uri.EscapeDataString(nameTemplate.Render(locals))}={Uri.EscapeDataString(valueTemplate.Render(locals))}");
        }
        return sb.ToString();
    }

    private Task AddAuthentication(RequestModel requestModel, HttpRequestMessage request)
    {
        if (requestModel.AuthProvider.Kind == AuthenticationKind.Inherit)
            return AddAuthenticationCore(requestModel.SavedRequest.ParentCollection.AuthProvider, requestModel.Logs, request);
        else
            return AddAuthenticationCore(requestModel.AuthProvider, requestModel.Logs, request);
    }

    private async Task AddAuthenticationCore(AuthenticationProvider provider, ObservableCollection<LogRecord> logs, HttpRequestMessage request)
    {
        logs.Add(new LogRecord()
        {
            Level = LogLevel.Info,
            Message = "Adding authorization",
            Timestamp = DateTimeOffset.UtcNow,
            Properties = new ObservableCollection<LogProperty>()
            {
                new LogProperty() { PropertyName = "Kind", PropertyValue = provider.Kind.ToString() }
            }
        });
        switch (provider.Kind)
        {
            case AuthenticationKind.Basic:
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", GetBasicAuthParameter(provider.Basic));
                break;
            case AuthenticationKind.Bearer:
                request.Headers.Authorization = new AuthenticationHeaderValue(provider.Bearer.Scheme, provider.Bearer.Parameter);
                break;
            case AuthenticationKind.Oauth2:
                var token = await _tokenHelper.GetToken(provider.Oauth2);
                if (_settings.AreLogsDetailed)
                {
                    logs.Add(new LogRecord()
                    {
                        Level = LogLevel.Debug,
                        Message = "Oauth2 token added",
                        Timestamp = DateTimeOffset.UtcNow,
                        Properties = new ObservableCollection<LogProperty>()
                        {
                            new LogProperty() { PropertyName = "token", PropertyValue = token }
                        }
                    });
                }
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                break;
        }
    }

    private string GetBasicAuthParameter(BasicAuthenticationModel model)
    {
        var concatenatedAndEscaped = $"{Uri.EscapeDataString(model.Username)}:{Uri.EscapeDataString(model.Password)}";
        var base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(concatenatedAndEscaped));
        return base64Encoded;
    }

    private async Task ParseResponseIntoModel(HttpResponseMessage response, RequestModel requestModel)
    {
        
        requestModel.ResponseStatus = $"HTTP {response.Version} {(int)response.StatusCode} {response.ReasonPhrase}";
        requestModel.ResponseStatusShort = $"[{response.StatusCode}]";
        var responseBody = await response.Content.ReadAsStringAsync();
        requestModel.ResponseBody.Source = responseBody;
        requestModel.ResponseBody.Type = BodyType.Text;
        requestModel.ResponseSize = Encoding.UTF8.GetBytes(responseBody).LongLength;
        requestModel.UnsavedChangesIndicatorVisibility = System.Windows.Visibility.Visible;
        requestModel.ResponseHeaders.Headers.Clear();
        foreach (var item in response.Headers)
        {
            requestModel.ResponseHeaders.Headers.Add(new HeaderItemModel(item.Key, string.Join(";", item.Value)));
        }
        foreach (var item in response.Content.Headers)
        {
            requestModel.ResponseHeaders.Headers.Add(new HeaderItemModel(item.Key, string.Join(";", item.Value)));
        }
    }

    public async Task RunRequest(RequestModel requestModel)
    {
        try
        {
            using var pooledClient = _clientPool.LeaseItem();

            requestModel.Logs.Clear();

            var requestMessage = await ConstructRequest(requestModel);

            var response = await pooledClient.Object.SendAsync(requestMessage);

            await ParseResponseIntoModel(response, requestModel);
        }
        catch (Exception ex)
        {
            requestModel.Logs.Add(new LogRecord()
            {
                Level = LogLevel.Error,
                Message = "Error while performing request",
                Properties = new ObservableCollection<LogProperty>()
                {
                    new LogProperty()
                    {
                        PropertyName = "Message",
                        PropertyValue = ex.Message
                    },
                    new LogProperty()
                    {
                        PropertyName = "Stack Trace",
                        PropertyValue = ex.StackTrace,
                    },
                    new LogProperty()
                    {
                        PropertyName = "Source",
                        PropertyValue = ex.Source,
                    },
                },
                Timestamp = DateTimeOffset.UtcNow
            });
            requestModel.ResponseStatus = "Error fetching response";
            requestModel.ResponseStatusShort = "[Error]";
        }
    }

    public void Dispose()
    {
        _clientPool?.Dispose();
    }
}
