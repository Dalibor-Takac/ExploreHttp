using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace ExploreHttp.Models
{
    public partial class ApplicationViewModel : ObservableObject
    {
        private ObservableCollection<RequestModel> openRequests;
        private ObservableCollection<RequestCollection> collections;
        private UIState uIState;
        private AppSettings appSettings;

        public ObservableCollection<RequestModel> OpenRequests { get => openRequests; set => SetProperty(ref openRequests, value); }
        public ObservableCollection<RequestCollection> Collections { get => collections; set => SetProperty(ref collections, value); }
        public UIState UIState { get => uIState; set => SetProperty(ref uIState, value); }
        public AppSettings AppSettings { get => appSettings; set => SetProperty(ref appSettings, value); }

        public ApplicationViewModel()
        {
            OpenRequests = new ObservableCollection<RequestModel>();
            Collections = new ObservableCollection<RequestCollection>();
            UIState = new UIState();
            AppSettings = new AppSettings();
        }

        public ApplicationViewModel(SavedState state)
        {
            UIState = new UIState()
            {
                X = state.X,
                Y = state.Y,
                Width = state.Width,
                Height = state.Height,
                SeparatorPosition = new GridLength(state.SeparatorPosition)
            };

            if (Enum.TryParse<WindowState>(state.WindowState, out var result))
            {
                UIState.WindowState = result;
            }

            AppSettings = new AppSettings()
            {
                RequireValidServerCert = state.RequireValidServerCert,
                AreLogsDetailed = state.AreLogsDetailed,
                UserAgentString = state.UserAgentString
            };

            //TODO handle loading of known collections
            Collections = new ObservableCollection<RequestCollection>()
            {
                new RequestCollection()
                {
                    CollectionName = "Example collection",
                    SavedRequests = new ObservableCollection<SavedRequest>()
                    {
                        new SavedRequest()
                        {
                            Method = RequestMethod.Get,
                            Name = "Example request",
                            Url = "http://example.com"
                        },
                        new SavedRequest()
                        {
                            Method = RequestMethod.Post,
                            Name = "Example request",
                            Url = "http://example.com"
                        },
                        new SavedRequest()
                        {
                            Method = RequestMethod.Put,
                            Name = "Example request",
                            Url = "http://example.com"
                        },
                        new SavedRequest()
                        {
                            Method = RequestMethod.Delete,
                            Name = "Example request",
                            Url = "http://example.com"
                        },
                        new SavedRequest()
                        {
                            Method = RequestMethod.Options,
                            Name = "Example request",
                            Url = "http://example.com"
                        },
                    },
                    SavedEnvironments = new ObservableCollection<SavedEnvironment>()
                    {
                        new SavedEnvironment() { Name = "Test" },
                        new SavedEnvironment() { Name = "Prod" },
                    },
                    SelectedEnvironmentIndex = 0
                },
                new RequestCollection()
                {
                    CollectionName = "Example collection",
                    SavedRequests = new ObservableCollection<SavedRequest>()
                    {
                        new SavedRequest()
                        {
                            Method = RequestMethod.Get,
                            Name = "Example request",
                            Url = "http://example.com"
                        },
                        new SavedRequest()
                        {
                            Method = RequestMethod.Get,
                            Name = "Example request",
                            Url = "http://example.com"
                        },
                        new SavedRequest()
                        {
                            Method = RequestMethod.Get,
                            Name = "Example request",
                            Url = "http://example.com"
                        },
                        new SavedRequest()
                        {
                            Method = RequestMethod.Get,
                            Name = "Example request",
                            Url = "http://example.com"
                        },
                        new SavedRequest()
                        {
                            Method = RequestMethod.Get,
                            Name = "Example request",
                            Url = "http://example.com"
                        },
                    },
                    SavedEnvironments = new ObservableCollection<SavedEnvironment>()
                    {
                        new SavedEnvironment() { Name = "Test" },
                        new SavedEnvironment() { Name = "Prod" },
                    },
                    SelectedEnvironmentIndex = 0,
                    IsExpanded = false
                },
                new RequestCollection()
                {
                    CollectionName = "Example collection",
                    SavedRequests = new ObservableCollection<SavedRequest>()
                    {
                        new SavedRequest()
                        {
                            Method = RequestMethod.Get,
                            Name = "Example request",
                            Url = "http://example.com"
                        },
                        new SavedRequest()
                        {
                            Method = RequestMethod.Get,
                            Name = "Example request",
                            Url = "http://example.com"
                        },
                        new SavedRequest()
                        {
                            Method = RequestMethod.Get,
                            Name = "Example request",
                            Url = "http://example.com"
                        },
                        new SavedRequest()
                        {
                            Method = RequestMethod.Get,
                            Name = "Example request",
                            Url = "http://example.com"
                        },
                        new SavedRequest()
                        {
                            Method = RequestMethod.Get,
                            Name = "Example request",
                            Url = "http://example.com"
                        },
                    },
                    SavedEnvironments = new ObservableCollection<SavedEnvironment>()
                    {
                        new SavedEnvironment() { Name = "Test" },
                        new SavedEnvironment() { Name = "Prod" },
                    },
                    SelectedEnvironmentIndex = 1
                },
            };
            //TODO handle loading of open requests
            OpenRequests = new ObservableCollection<RequestModel>()
            {
                new RequestModel()
                {
                    Name = "Example",
                    Method = RequestMethod.Get,
                    Url = "http://example.com/",
                    UnsavedChangesIndicatorVisibility = Visibility.Visible,
                    RequestHeaders = new HeaderCollection()
                    {
                        Headers = new ObservableCollection<HeaderItemModel>()
                        {
                            new HeaderItemModel("Content-Type", "application/json"),
                            new HeaderItemModel("Autorization", "Bearer aabbccdd")
                        },
                        IsEditable = true
                    },
                    RequestBody = new BodyProvider()
                    {
                        ContentType = "text/plain",
                        Type = BodyType.Text,
                        Source = "{}",
                        Size = 4
                    },
                    ResponseStatus = "HTTP1.1 200 OK",
                    ResponseDuration = TimeSpan.FromMilliseconds(12345),
                    ResponseBody = new BodyProvider()
                    {
                        Size = 1234567L,
                        Source = "{}",
                        Type = BodyType.Text
                    }
                },
                new RequestModel()
                {
                    Name = string.Empty,
                    Method = RequestMethod.Get,
                    Url = null,
                    UnsavedChangesIndicatorVisibility = Visibility.Visible,
                    RequestHeaders = new HeaderCollection()
                    {
                        Headers = new ObservableCollection<HeaderItemModel>()
                        {
                            new HeaderItemModel("Content-Type", "application/json"),
                            new HeaderItemModel("Autorization", "Bearer aabbccdd")
                        },
                        IsEditable = true
                    },
                    RequestBody = new BodyProvider()
                    {
                        ContentType = "text/plain",
                        Type = BodyType.Text,
                        Source = "{}",
                        Size = 4
                    }
                }
            };
        }

        public void ToSettings(SavedState state)
        {
            state.X = UIState.X;
            state.Y = UIState.Y;
            state.Width = UIState.Width;
            state.Height = UIState.Height;
            state.WindowState = UIState.WindowState.ToString();
            state.SeparatorPosition = UIState.SeparatorPosition.Value;

            state.AreLogsDetailed = AppSettings.AreLogsDetailed;
            state.RequireValidServerCert = AppSettings.RequireValidServerCert;
            state.UserAgentString = AppSettings.UserAgentString;
        }
    }

    public partial class UIState : ObservableObject
    {
        private double x;
        private double y;
        private double width;
        private double height;
        private WindowState windowState;
        private GridLength separatorPosition;

        public double X { get => x; set => SetProperty(ref x, value); }
        public double Y { get => y; set => SetProperty(ref y, value); }
        public double Width { get => width; set => SetProperty(ref width, value); }
        public double Height { get => height; set => SetProperty(ref height, value); }
        public WindowState WindowState { get => windowState; set => SetProperty(ref windowState, value); }
        public GridLength SeparatorPosition { get => separatorPosition; set => SetProperty(ref separatorPosition, value); }
    }

    public partial class AppSettings : ObservableObject
    {
        private bool requireValidServerCert;
        private bool areLogsDetailed;
        private string userAgentString;

        public bool RequireValidServerCert { get => requireValidServerCert; set => SetProperty(ref requireValidServerCert, value); }
        public bool AreLogsDetailed { get => areLogsDetailed; set => SetProperty(ref areLogsDetailed, value); }
        public string UserAgentString { get => userAgentString; set => SetProperty(ref userAgentString, value); }

        public AppSettings Clone()
        {
            var clone = new AppSettings()
            {
                RequireValidServerCert = this.RequireValidServerCert,
                AreLogsDetailed = this.AreLogsDetailed,
                UserAgentString = this.UserAgentString
            };

            return clone;
        }
    }
}
