using ExploreHttp.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace ExploreHttp.Services;
public class LoggingHttpMessageHandler: DelegatingHandler
{
    private readonly AppSettings _settings;
    private static readonly RemoteCertificateValidationCallback DefaultServerCertValidationCallback = ServicePointManager.ServerCertificateValidationCallback;

    private static bool BypassServerCertValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors) => true;

    public LoggingHttpMessageHandler(AppSettings settings)
    {
        _settings = settings;
        _settings.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(AppSettings.RequireValidServerCert))
            {
                ServicePointManager.ServerCertificateValidationCallback = _settings.RequireValidServerCert
                    ? DefaultServerCertValidationCallback
                    : BypassServerCertValidationCallback;
            }
        };
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var requestModel = request.Properties[nameof(RequestModel)] as RequestModel;

        requestModel.Logs.Add(new LogRecord()
        {
            Level = LogLevel.Info,
            Message = "Starting request log",
            Timestamp = DateTimeOffset.UtcNow,
        });

        if (_settings.AreLogsDetailed)
        {
            requestModel.Logs.Add(new LogRecord()
            {
                Level = LogLevel.Debug,
                Message = "Request Headers",
                Properties = new ObservableCollection<LogProperty>(request.HeadersToLogProperties()),
                Timestamp = DateTimeOffset.UtcNow
            });
            if (request.Content is not null)
            {
                requestModel.Logs.Add(new LogRecord()
                {
                    Level = LogLevel.Debug,
                    Message = "Request Body",
                    Properties = new ObservableCollection<LogProperty>()
                    {
                        new LogProperty()
                        {
                            PropertyName = "content",
                            PropertyValue = await request.Content.ReadAsStringAsync()
                        }
                    },
                    Timestamp = DateTimeOffset.UtcNow
                });
            }
            else
            {
                requestModel.Logs.Add(new LogRecord()
                {
                    Level = LogLevel.Debug,
                    Message = "No request body present",
                    Timestamp = DateTimeOffset.UtcNow
                });
            }
        }
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var response = await base.SendAsync(request, cancellationToken);

        stopwatch.Stop();

        requestModel.ResponseDuration = stopwatch.Elapsed;
        
        requestModel.Logs.Add(new LogRecord()
        {
            Level = LogLevel.Info,
            Message = "Request sent, receiving response",
            Properties = new ObservableCollection<LogProperty>()
            {
                new LogProperty()
                {
                    PropertyName = "Duration",
                    PropertyValue = stopwatch.Elapsed.ToString()
                }
            },
            Timestamp = DateTimeOffset.UtcNow
        });

        if (_settings.AreLogsDetailed)
        {
            requestModel.Logs.Add(new LogRecord()
            {
                Level = LogLevel.Debug,
                Message = "Response Headers",
                Properties = new ObservableCollection<LogProperty>(response.HeadersToLogProperties()),
                Timestamp = DateTimeOffset.UtcNow
            });
            if (response.Content is not null)
            {
                requestModel.Logs.Add(new LogRecord()
                {
                    Level = LogLevel.Debug,
                    Message = "Response Body",
                    Properties = new ObservableCollection<LogProperty>()
                    {
                        new LogProperty()
                        {
                            PropertyName = "content",
                            PropertyValue = await response.Content.ReadAsStringAsync()
                        }
                    },
                    Timestamp = DateTimeOffset.UtcNow
                });
            }
            else
            {
                requestModel.Logs.Add(new LogRecord()
                {
                    Level = LogLevel.Debug,
                    Message = "No response body",
                    Timestamp = DateTimeOffset.UtcNow
                });
            }
        }

        return response;
    }
}
