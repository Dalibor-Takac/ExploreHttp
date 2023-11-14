using ExploreHttp.Models;
using Newtonsoft.Json;

namespace ExploreHttp.Services.PersistanceModels;
public class Request
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public RequestMethod Method { get; set; }
    public string Url { get; set; }
    public List<KeyValuePairWithEnable> QueryString { get; set; }
    public List<KeyValuePairWithEnable> RequestHeaders { get; set; }
    public ContentProvider RequestBody { get; set; }
    public string ResponseStatus { get; set; }
    public List<KeyValuePairWithEnable> ResponseHeaders { get; set; }
    public ContentProvider ResponseBody { get; set; }
    public TimeSpan ResponseDuration { get; set; }
    public long ResponseSize { get; set; }
    public List<LogEvent> Logs { get; set; }
    public AuthenticationKind AuthKind { get; set; }
    public BasicAuth Basic { get; set; }
    public BearerAuth Bearer { get; set; }
    public Oauth2Auth Oauth2 { get; set; }
}

public class KeyValuePairWithEnable
{
    public bool IsEnabled { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
}

public class ContentProvider
{
    public BodyType Kind { get; set; }
    public string Source { get; set; }
}

public class LogEvent
{
    public LogLevel Level { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public string Message { get; set; }
    public Dictionary<string, string> Properties { get; set; }
}