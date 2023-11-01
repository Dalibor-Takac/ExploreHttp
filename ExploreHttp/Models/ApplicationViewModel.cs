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

        public ObservableCollection<RequestModel> OpenRequests { get => openRequests; set => SetProperty(ref openRequests, value); }
        public ObservableCollection<RequestCollection> Collections { get => collections; set => SetProperty(ref collections, value); }
        public UIState UIState { get => uIState; set => SetProperty(ref uIState, value); }

        public ApplicationViewModel()
        {
            OpenRequests = new ObservableCollection<RequestModel>();
            Collections = new ObservableCollection<RequestCollection>();
            UIState = new UIState();
        }

        public ApplicationViewModel(SavedState state)
        {
            UIState = new UIState()
            {
                X = state.X,
                Y = state.Y,
                Width = state.Width,
                Height = state.Height
            };

            if (Enum.TryParse<WindowState>(state.WindowState, out var result))
            {
                UIState.WindowState = result;
            }

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
        }
    }

    public partial class UIState : ObservableObject
    {
        private double x;
        private double y;
        private double width;
        private double height;
        private WindowState windowState;

        public double X { get => x; set => SetProperty(ref x, value); }
        public double Y { get => y; set => SetProperty(ref y, value); }
        public double Width { get => width; set => SetProperty(ref width, value); }
        public double Height { get => height; set => SetProperty(ref height, value); }
        public WindowState WindowState { get => windowState; set => SetProperty(ref windowState, value); }
    }
}
