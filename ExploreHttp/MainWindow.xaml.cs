using ExploreHttp.Models;
using ExploreHttp.Services;
using Microsoft.Win32;
using System.Collections.Specialized;
using System.IO;
using System.Windows;

namespace ExploreHttp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ApplicationViewModel Vm { get; set; }
        public MainWindow()
        {
            if (SavedState.Default.KnownCollections is null)
                SavedState.Default.KnownCollections = new StringCollection();

            Vm = new ApplicationViewModel(SavedState.Default);
            DataContext = Vm;
            InitializeComponent();
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Vm.ToSettings(SavedState.Default);
            SavedState.Default.Save();
        }

        private void NewCollection_Click(object sender, RoutedEventArgs e)
        {
            var newCollection = CollectionEditorWindow.OpenModal(this);
            if (newCollection != null)
                Vm.Collections.Add(newCollection);
        }

        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            var newSettings = SettingsWindow.OpenModal(this, Vm.AppSettings.Clone());
            if (newSettings != null)
                Vm.AppSettings = newSettings;
        }

        private void CloseRequest_Click(object sender, RoutedEventArgs e)
        {
            var request = (sender as FrameworkElement).DataContext as RequestModel;
            Vm.OpenRequests.Remove(request);
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow.OpenDialog(this);
        }

        private void OpenCollection_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "Request Collection|*.reqcol|All Files|*.*";
            dlg.CheckFileExists = true;
            dlg.CheckPathExists = true;
            if (dlg.ShowDialog(this).GetValueOrDefault())
            {
                if (File.Exists(dlg.FileName) && !SavedState.Default.KnownCollections.Contains(dlg.FileName))
                {
                    SavedState.Default.KnownCollections.Add(dlg.FileName);
                    SavedState.Default.Save();

                    var loader = new CollectionLoader(dlg.FileName);

                    var metadata = loader.ReadMetadata();
                    var requestCollection = ModelConverter.FromStorage(metadata);
                    requestCollection.Loader = loader;

                    Vm.Collections.Add(requestCollection);
                }
            }
        }
    }
}
