using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace ExploreHttp.Models
{
    public partial class RequestModel : ObservableObject
    {
        private string name;
        private RequestMethod method;
        private string url;
        private HeaderCollection requestHeaders;
        private BodyProvider requestBody;
        private Visibility unsavedChangesIndicatorVisibility;
        private string responseStatus;
        private TimeSpan responseDuration;
        private HeaderCollection responseHeaders;
        private BodyProvider responseBody;

        public string Name { get => name; set => SetProperty(ref name, value); }
        public RequestMethod Method { get => method; set => SetProperty(ref method, value); }
        public string Url { get => url; set => SetProperty(ref url, value); }
        public HeaderCollection RequestHeaders { get => requestHeaders; set => SetProperty(ref requestHeaders, value); }
        public BodyProvider RequestBody { get => requestBody; set => SetProperty(ref requestBody, value); }
        public Visibility UnsavedChangesIndicatorVisibility { get => unsavedChangesIndicatorVisibility; set => SetProperty(ref unsavedChangesIndicatorVisibility, value); }
        public string ResponseStatus { get => responseStatus; set => SetProperty(ref responseStatus, value); }
        public TimeSpan ResponseDuration { get => responseDuration; set => SetProperty(ref responseDuration, value); }
        public HeaderCollection ResponseHeaders { get => responseHeaders; set => SetProperty(ref responseHeaders, value); }
        public BodyProvider ResponseBody { get => responseBody; set => SetProperty(ref responseBody, value); }

        public RequestModel()
        {
            RequestHeaders = new HeaderCollection();
            RequestBody = new BodyProvider();
            ResponseHeaders = new HeaderCollection();
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
        private string contentType;
        private long size;
        private string source;

        public BodyType Type { get => type; set => SetProperty(ref type, value); }
        public string ContentType { get => contentType; set => SetProperty(ref contentType, value); }
        public long Size { get => size; set => SetProperty(ref size, value); }
        public string Source { get => source; set => SetProperty(ref source, value); }
    }
}
