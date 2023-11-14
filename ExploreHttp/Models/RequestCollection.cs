using CommunityToolkit.Mvvm.ComponentModel;
using ExploreHttp.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace ExploreHttp.Models;

public partial class RequestCollection : ObservableObject
{
    private string collectionName;
    private CollectionKind kind;
    private string source;
    private Visibility unsavedChangesIndicatorVisibility;
    private ObservableCollection<SavedRequest> savedRequests;
    private ObservableCollection<SavedEnvironment> savedEnvironments;
    private int selectedEnvironmentIndex;
    private bool isExpanded;
    private AuthenticationProvider authProvider;

    public string CollectionName { get => collectionName; set => SetProperty(ref collectionName, value); }
    public CollectionKind Kind { get => kind; set => SetProperty(ref kind, value); }
    public string Source { get => source; set => SetProperty(ref source, value); }
    public Visibility UnsavedChangesIndicatorVisibility { get => unsavedChangesIndicatorVisibility; set => SetProperty(ref unsavedChangesIndicatorVisibility, value); }
    public ObservableCollection<SavedRequest> SavedRequests { get => savedRequests; set => SetProperty(ref savedRequests, value); }
    public ObservableCollection<SavedEnvironment> SavedEnvironments { get => savedEnvironments; set => SetProperty(ref savedEnvironments, value); }
    public int SelectedEnvironmentIndex { get => selectedEnvironmentIndex; set => SetProperty(ref selectedEnvironmentIndex, value); }
    public bool IsExpanded { get => isExpanded; set => SetProperty(ref isExpanded, value); }
    public AuthenticationProvider AuthProvider { get => authProvider; set => SetProperty(ref authProvider, value); }

    public RequestCollection()
    {
        SavedRequests = new ObservableCollection<SavedRequest>();
        SavedEnvironments = new ObservableCollection<SavedEnvironment>();
        IsExpanded = true;
        AuthProvider = new AuthenticationProvider();
    }

    public CollectionLoader Loader { get; set; }

    public RequestCollection Clone()
    {
        var result = new RequestCollection()
        {
            CollectionName = CollectionName,
            Kind = Kind,
            Source = Source,
            UnsavedChangesIndicatorVisibility = UnsavedChangesIndicatorVisibility,
            SavedRequests = new ObservableCollection<SavedRequest>(SavedRequests),
            SavedEnvironments = new ObservableCollection<SavedEnvironment>(SavedEnvironments),
            SelectedEnvironmentIndex = SelectedEnvironmentIndex,
            IsExpanded = IsExpanded,
            Loader = Loader
        };
        return result;
    }

    public void SyncWithOther(RequestCollection collection)
    {
        CollectionName = collection.CollectionName;
        Kind = collection.Kind;
        Source = collection.Source;
        SavedRequests.Clear();
        foreach (var r in collection.SavedRequests)
            SavedRequests.Add(r);
        SavedEnvironments.Clear();
        foreach (var env in collection.SavedEnvironments)
            SavedEnvironments.Add(env);
        SelectedEnvironmentIndex = collection.SelectedEnvironmentIndex;
        IsExpanded = collection.IsExpanded;
        Loader = collection.Loader;
        UnsavedChangesIndicatorVisibility = collection.UnsavedChangesIndicatorVisibility;
    }
}

public partial class SavedRequest : ObservableObject
{
    private Guid id;
    private string name;
    private RequestMethod method;
    private string url;
    private string operationId;
    private RequestCollection parentCollection;

    public Guid Id { get => id; set => SetProperty(ref id, value); }
    public string Name { get => name; set => SetProperty(ref name, value); }
    public RequestMethod Method { get => method; set => SetProperty(ref method, value); }
    public string Url { get => url; set => SetProperty(ref url, value); }
    public string OperationId { get => operationId; set => SetProperty(ref operationId, value); }
    public RequestCollection ParentCollection { get => parentCollection; set => SetProperty(ref parentCollection, value); }

    public SavedRequest(RequestCollection parentCollection)
    {
        id = Guid.NewGuid();
        this.parentCollection = parentCollection;
    }
}

public partial class SavedEnvironment : ObservableObject
{
    private Guid id;
    private string name;
    private ObservableCollection<EnvironmentVariable> variables;

    public Guid Id { get => id; set => SetProperty(ref id, value); }
    public string Name { get => name; set => SetProperty(ref name, value); }
    public ObservableCollection<EnvironmentVariable> Variables { get => variables; set => SetProperty(ref variables, value); }

    public SavedEnvironment()
    {
        Id = Guid.NewGuid();
        Variables = new ObservableCollection<EnvironmentVariable>();
    }

    public SavedEnvironment Clone()
    {
        var clone = new SavedEnvironment();
        clone.Id = Id;
        clone.Name = Name;
        clone.Variables = new ObservableCollection<EnvironmentVariable>(Variables);
        return clone;
    }
}

public partial class EnvironmentVariable : ObservableObject
{
    private string name;
    private string _value;
    private bool isEnabled;

    public string Name { get => name; set => SetProperty(ref name, value); }
    public string Value { get => _value; set => SetProperty(ref _value, value); }
    public bool IsEnabled { get => isEnabled; set => SetProperty(ref isEnabled, value); }
}
