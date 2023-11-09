using ExploreHttp.Models;
using ExploreHttp.Services;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace ExploreHttp
{
    /// <summary>
    /// Interaction logic for CollectionEditorWindow.xaml
    /// </summary>
    public partial class CollectionEditorWindow : Window
    {
        public CollectionEditorWindow()
        {
            InitializeComponent();
        }

        private RequestCollection Vm => (RequestCollection)DataContext;

        private void EditEnvironment_Click(object sender, RoutedEventArgs e)
        {
            var senderButton = sender as Button;
            var existingEnvironment = senderButton.DataContext as SavedEnvironment;
            var updated = EnvironmentEditorWindow.OpenModal(this, existingEnvironment.Clone());
            if (updated != null)
            {
                Vm.SavedEnvironments.Remove(existingEnvironment);
                Vm.SavedEnvironments.Add(updated);
            }
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void AddEnvironment_Click(object sender, RoutedEventArgs e)
        {
            var newEnvironment = EnvironmentEditorWindow.OpenModal(this);
            if (newEnvironment != null)
                Vm.SavedEnvironments.Add(newEnvironment);
        }

        private void ChangeSource_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.CheckFileExists = true;
            dlg.CheckPathExists = true;
            dlg.Filter = "OpenAPI specs (JSON)|*.json|OpenAPI specs (YAML)|*.yml;*.yaml|All Files|*.*";
            dlg.FilterIndex = 1;
            dlg.Multiselect = false;
            dlg.Title = "Open local OpenAPI spec file";
            if (dlg.ShowDialog(this).GetValueOrDefault())
            {
                Vm.Source = dlg.FileName;
                RefreshEndpoints_Click(sender, e);
            }
        }

        private async void RefreshEndpoints_Click(object sender, RoutedEventArgs e)
        {
            var openApiImporter = new OpenApiImporter(Vm.Source, AppSettings);
            await openApiImporter.RefreshImport(Vm);
        }

        private void DuplicateEnvironment_Click(object sender, RoutedEventArgs e)
        {
            var sourceEnvironment = (sender as Button).DataContext as SavedEnvironment;
            var newEnvironment = new SavedEnvironment()
            {
                Name = sourceEnvironment.Name + " Copy",
                Variables = new ObservableCollection<EnvironmentVariable>(sourceEnvironment.Variables
                    .Select(x => new EnvironmentVariable()
                    {
                        IsEnabled = x.IsEnabled,
                        Name = x.Name,
                        Value = x.Value
                    }))
            };
            Vm.SavedEnvironments.Add(newEnvironment);
        }

        private void DeleteEnvironment_Click(object sender, RoutedEventArgs e)
        {
            var sourceEnvironment = (sender as Button).DataContext as SavedEnvironment;

            if (Vm.SelectedEnvironmentIndex < Vm.SavedEnvironments.Count)
            {
                var selectedEnvironment = Vm.SavedEnvironments[Vm.SelectedEnvironmentIndex];
                Vm.SavedEnvironments.Remove(sourceEnvironment);
                Vm.SelectedEnvironmentIndex = Vm.SavedEnvironments.IndexOf(selectedEnvironment);
                if (Vm.SelectedEnvironmentIndex < 0)
                    Vm.SelectedEnvironmentIndex = 0;
            }
            else
            {
                Vm.SavedEnvironments.Remove(sourceEnvironment);
            }
        }

        public AppSettings AppSettings { get; set; }

        public static RequestCollection OpenModal(Window parent, AppSettings settings, RequestCollection editingInstance = default)
        {
            var vm = editingInstance ?? new RequestCollection();
            var dlg = new CollectionEditorWindow();
            dlg.Owner = parent;
            dlg.DataContext = vm;
            dlg.AppSettings = settings;
            if (editingInstance != null)
                dlg.Title = "Explore HTTP - Edit Collection";

            if (dlg.ShowDialog().GetValueOrDefault())
                return vm;

            return null;
        }
    }
}
