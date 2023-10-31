using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace ExploreHttp.Models
{
    public partial class RequestCollection : ObservableObject
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

    public partial class SavedRequest : ObservableObject
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public RequestMethod Method { get; set; }
        public string Url { get; set; }
    }

    public partial class SavedEnvironment : ObservableObject
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ObservableCollection<EnvironmentVariable> Variables { get; set; }

        public SavedEnvironment()
        {
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
        public string Name { get; set; }
        public string Value { get; set; }
        public bool IsEnabled { get; set; }
    }
}
