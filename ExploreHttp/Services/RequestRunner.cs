using DotLiquid;
using ExploreHttp.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;

namespace ExploreHttp.Services;
public class RequestRunner : IDisposable
{
    private readonly ObjectPool<HttpClient> _clientPool;
    private readonly AppSettings _settings;

    public RequestRunner(AppSettings settings)
    {
        _settings = settings;
        _clientPool = new ObjectPool<HttpClient>(
            settings.RequestPoolSize,
            () => new HttpClient(new LoggingHttpMessageHandler(_settings) { InnerHandler = new HttpClientHandler() }),
            client => client.CancelPendingRequests());
    }

    private Hash EnvironmentToLocals(RequestModel requestModel)
    {
        var result = new Hash();

        if (requestModel.SavedRequest.ParentCollection.SavedEnvironments.Count > requestModel.SavedRequest.ParentCollection.SelectedEnvironmentIndex)
        {
            var environment = requestModel.SavedRequest.ParentCollection.SavedEnvironments[requestModel.SavedRequest.ParentCollection.SelectedEnvironmentIndex];

            foreach (var item in environment.Variables)
            {
                result.Add(item.Name, item.Value);
            }
        }

        return result;
    }

    private HttpRequestMessage ConstructRequest(RequestModel requestModel)
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

        var locals = EnvironmentToLocals(requestModel);

        var uriTemplate = Template.Parse(requestModel.Url);
        result.RequestUri = new Uri(uriTemplate.Render(locals), UriKind.Absolute);
        
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
            if (!result.Headers.TryAddWithoutValidation(header.Key, header.Value))
            {
                result.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        result.Properties.Add(nameof(RequestModel), requestModel);

        return result;
    }

    private async Task ParseResponseIntoModel(HttpResponseMessage response, RequestModel requestModel)
    {
        
        requestModel.ResponseStatus = $"HTTP {response.Version} {(int)response.StatusCode} {response.ReasonPhrase}";
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
            var pooledClient = _clientPool.LeaseItem();

            var requestMessage = ConstructRequest(requestModel);

            var response = await pooledClient.SendAsync(requestMessage);

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
                }
            });
            requestModel.ResponseStatus = "Error fetching response";
        }
    }

    public void Dispose()
    {
        if (_clientPool is not null)
            _clientPool.Dispose();
    }
}
