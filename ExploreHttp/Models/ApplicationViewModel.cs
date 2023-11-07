using CommunityToolkit.Mvvm.ComponentModel;
using ExploreHttp.Services;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Windows;

namespace ExploreHttp.Models;

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
            UserAgentString = state.UserAgentString,
            RequestPoolSize = state.RequestPoolSize
        };

        Collections = new ObservableCollection<RequestCollection>(EnumerateKnownCollections(state.KnownCollections));
        OpenRequests = new ObservableCollection<RequestModel>();
    }

    private IEnumerable<RequestCollection> EnumerateKnownCollections(StringCollection knownCollections)
    {
        foreach (var knownCollectionItem in knownCollections)
        {
            if (File.Exists(knownCollectionItem))
            {
                var loader = new CollectionLoader(knownCollectionItem);
                var metadata = loader.ReadMetadata();
                var collection = ModelConverter.FromStorage(metadata);
                collection.Loader = loader;
                yield return collection;
            }
        }
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
        state.RequestPoolSize = AppSettings.RequestPoolSize;

        state.KnownCollections.Clear();
        foreach (var col in Collections)
        {
            state.KnownCollections.Add(col.Loader.FileName);
        }
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
    private int requestPoolSize;

    public bool RequireValidServerCert { get => requireValidServerCert; set => SetProperty(ref requireValidServerCert, value); }
    public bool AreLogsDetailed { get => areLogsDetailed; set => SetProperty(ref areLogsDetailed, value); }
    public string UserAgentString { get => userAgentString; set => SetProperty(ref userAgentString, value); }
    public int RequestPoolSize { get => requestPoolSize; set => SetProperty(ref requestPoolSize, value); }

    public AppSettings Clone()
    {
        var clone = new AppSettings()
        {
            RequireValidServerCert = this.RequireValidServerCert,
            AreLogsDetailed = this.AreLogsDetailed,
            UserAgentString = this.UserAgentString,
            RequestPoolSize = this.RequestPoolSize
        };

        return clone;
    }
}
