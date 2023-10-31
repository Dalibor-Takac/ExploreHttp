using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace ExploreHttp.Models
{
    public partial class RequestModel : ObservableObject
    {
        public string Name { get; set; }
        public RequestMethod Method { get; set; }
        public string Url { get; set; }
        public HeaderCollection RequestHeaders { get; set; }
        public BodyProvider RequestBody { get; set; }
        public Visibility UnsavedChangesIndicatorVisibility { get; set; }
        public string ResponseStatus { get; set; }
        public TimeSpan ResponseDuration { get; set; }
        public HeaderCollection ResponseHeaders { get; set; }
        public BodyProvider ResponseBody { get; set; }

        public RequestModel()
        {
            RequestHeaders = new HeaderCollection();
            RequestBody = new BodyProvider();
            ResponseHeaders = new HeaderCollection();
        }
    }

    public partial class HeaderItemModel : ObservableObject
    {
        public string HeaderName { get; set; }
        public string HeaderValue { get; set; }
        public bool IsEnabled { get; set; }

        public HeaderItemModel(string name, string value)
        {
            HeaderName = name;
            HeaderValue = value;
            IsEnabled = true;
        }
    }

    public partial class HeaderCollection : ObservableObject
    {
        public ObservableCollection<HeaderItemModel> Headers { get; set; }
        public bool IsEditable { get; set; }

        public HeaderCollection()
        {
            Headers = new ObservableCollection<HeaderItemModel>();
        }
    }

    public partial class BodyProvider : ObservableObject
    {
        public BodyType Type { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }
        public string Source { get; set; }
    }
}
