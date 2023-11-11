using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;

namespace ExploreHttp.Models;

public partial class RequestModel : ObservableObject
{
    private Guid id;
    private string name;
    private RequestMethod method;
    private string url;
    private QueryStringModel queryString;
    private HeaderCollection requestHeaders;
    private AuthenticationProvider authProvider;
    private BodyProvider requestBody;
    private Visibility unsavedChangesIndicatorVisibility;
    private string responseStatus;
    private TimeSpan responseDuration;
    private long responseSize;
    private HeaderCollection responseHeaders;
    private BodyProvider responseBody;
    private ObservableCollection<LogRecord> logs;
    private SavedRequest savedRequest;

    public Guid Id { get => id; set => SetProperty(ref id, value); }
    public string Name { get => name; set => SetProperty(ref name, value); }
    public RequestMethod Method { get => method; set => SetProperty(ref method, value); }
    public string Url { get => url; set => SetProperty(ref url, value); }
    public QueryStringModel QueryString { get => queryString; set => SetProperty(ref queryString, value); }
    public HeaderCollection RequestHeaders { get => requestHeaders; set => SetProperty(ref requestHeaders, value); }
    public AuthenticationProvider AuthProvider { get => authProvider; set => SetProperty(ref authProvider, value); }
    public BodyProvider RequestBody { get => requestBody; set => SetProperty(ref requestBody, value); }
    public Visibility UnsavedChangesIndicatorVisibility { get => unsavedChangesIndicatorVisibility; set => SetProperty(ref unsavedChangesIndicatorVisibility, value); }
    public string ResponseStatus { get => responseStatus; set => SetProperty(ref responseStatus, value); }
    public TimeSpan ResponseDuration { get => responseDuration; set => SetProperty(ref responseDuration, value); }
    public long ResponseSize { get => responseSize; set => SetProperty(ref responseSize, value); }
    public HeaderCollection ResponseHeaders { get => responseHeaders; set => SetProperty(ref responseHeaders, value); }
    public BodyProvider ResponseBody { get => responseBody; set => SetProperty(ref responseBody, value); }
    public ObservableCollection<LogRecord> Logs { get => logs; set => SetProperty(ref logs, value); }
    public SavedRequest SavedRequest { get => savedRequest; set => SetProperty(ref savedRequest, value); }

    public RequestModel(SavedRequest req)
    {
        Id = req.Id;
        Name = req.Name;
        Method = req.Method;
        Url = req.Url;
        QueryString = new QueryStringModel();
        RequestHeaders = new HeaderCollection() { IsEditable = true };
        AuthProvider = new AuthenticationProvider();
        RequestBody = new BodyProvider();
        ResponseBody = new BodyProvider();
        ResponseHeaders = new HeaderCollection();
        Logs = new ObservableCollection<LogRecord>();
        SavedRequest = req;

        PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName != nameof(UnsavedChangesIndicatorVisibility))
                UnsavedChangesIndicatorVisibility = Visibility.Visible;
        };
        QueryString.PropertyChanged += (sender, args) => { UnsavedChangesIndicatorVisibility = Visibility.Visible; };
        RequestHeaders.PropertyChanged += (sender, args) => { UnsavedChangesIndicatorVisibility = Visibility.Visible; };
        AuthProvider.PropertyChanged += (sender, args) => { UnsavedChangesIndicatorVisibility = Visibility.Visible; };
        RequestBody.PropertyChanged += (sender, args) => { UnsavedChangesIndicatorVisibility = Visibility.Visible; };
        ResponseHeaders.PropertyChanged += (sender, args) => { UnsavedChangesIndicatorVisibility = Visibility.Visible; };
        ResponseBody.PropertyChanged += (sender, args) => { UnsavedChangesIndicatorVisibility = Visibility.Visible; };
        Logs.CollectionChanged += (sender, args) => { UnsavedChangesIndicatorVisibility = Visibility.Visible; };
    }
}

public partial class HeaderItemModel : ObservableObject
{
    private string headerName;
    private string headerValue;
    private bool isEnabled;

    public string HeaderName { get => headerName; set => SetProperty(ref headerName, value); }
    public string HeaderValue { get => headerValue; set => SetProperty(ref headerValue, value); }
    public bool IsEnabled { get => isEnabled; set => SetProperty(ref isEnabled, value); }

    public HeaderItemModel(string name, string value)
    {
        HeaderName = name;
        HeaderValue = value;
        IsEnabled = true;
    }

    public HeaderItemModel()
    {
        isEnabled = true;
    }
}

public partial class HeaderCollection : ObservableObject
{
    private ObservableCollection<HeaderItemModel> headers;
    private bool isEditable;

    public ObservableCollection<HeaderItemModel> Headers { get => headers; set => SetProperty(ref headers, value); }
    public bool IsEditable { get => isEditable; set => SetProperty(ref isEditable, value); }

    public HeaderCollection()
    {
        Headers = new ObservableCollection<HeaderItemModel>();
    }
}

public partial class BodyProvider : ObservableObject
{
    private BodyType type;
    private string source;

    public BodyType Type { get => type; set => SetProperty(ref type, value); }
    public string Source { get => source; set => SetProperty(ref source, value); }
}

public partial class LogRecord : ObservableObject
{
    private LogLevel level;
    private DateTimeOffset timestamp;
    private string message;
    private ObservableCollection<LogProperty> properties;

    public LogLevel Level { get => level; set => SetProperty(ref level, value); }
    public DateTimeOffset Timestamp { get => timestamp; set => SetProperty(ref timestamp, value); }
    public string Message { get => message; set => SetProperty(ref message, value); }
    public ObservableCollection<LogProperty> Properties { get => properties; set => SetProperty(ref properties, value); }

    public LogRecord()
    {
        Properties = new ObservableCollection<LogProperty>();
    }
}

public enum LogLevel
{
    Fatal,
    Error,
    Warning,
    Info,
    Debug,
}

public partial class LogProperty : ObservableObject
{
    private string propertyName;
    private string propertyValue;

    public string PropertyName { get => propertyName; set => SetProperty(ref propertyName, value); }
    public string PropertyValue { get => propertyValue; set => SetProperty(ref propertyValue, value); }
}
