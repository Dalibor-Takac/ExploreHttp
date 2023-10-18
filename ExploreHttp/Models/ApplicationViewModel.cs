using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace ExploreHttp.Models
{
    [ObservableObject]
    public partial class ApplicationViewModel
    {
        public ObservableCollection<RequestModel> OpenRequests { get; set; }
        public ObservableCollection<RequestCollection> Collections { get; set; }
        public UIState UIState { get; set; }

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

    [ObservableObject]
    public partial class UIState
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public WindowState WindowState { get; set; }
    }
}
