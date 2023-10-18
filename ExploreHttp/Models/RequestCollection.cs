using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace ExploreHttp.Models
{
    [ObservableObject]
    public partial class RequestCollection
    {
        public string CollectionName { get; set; }
        public CollectionKind Kind { get; set; }
        public string Source { get; set; }
        public Visibility UnsavedChangesIndicatorVisibility { get; set; }
        public ObservableCollection<SavedRequest> SavedRequests { get; set; }
        public ObservableCollection<SavedEnvironment> SavedEnvironments { get; set; }
        public int SelectedEnvironmentIndex { get; set; }
        public bool IsExpanded { get; set; }

        public RequestCollection()
        {
            SavedRequests = new ObservableCollection<SavedRequest>();
            SavedEnvironments = new ObservableCollection<SavedEnvironment>();
            IsExpanded = true;
        }
    }

    [ObservableObject]
    public partial class SavedRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public RequestMethod Method { get; set; }
        public string Url { get; set; }
    }

    [ObservableObject]
    public partial class SavedEnvironment
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
