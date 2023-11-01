using ExploreHttp.Models;
using ExploreHttp.Services;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace ExploreHttp
{
    /// <summary>
    /// Interaction logic for RequestCollectionControl.xaml
    /// </summary>
    public partial class RequestCollectionControl : UserControl
    {
        public RequestCollectionControl()
        {
            InitializeComponent();
        }

        private RequestCollection Vm => (RequestCollection)DataContext;

        private void SaveCollection()
        {
            if (Vm.Loader is null)
            {
                var saveDlg = new SaveFileDialog();
                saveDlg.CheckPathExists = true;
                saveDlg.AddExtension = true;
                saveDlg.DefaultExt = "*.reqcol";
                saveDlg.Filter = "Request Collection|*.reqcol";
                if (saveDlg.ShowDialog().GetValueOrDefault() && !SavedState.Default.KnownCollections.Contains(saveDlg.FileName))
                {
                    var loader = new CollectionLoader(saveDlg.FileName);
                    Vm.Loader = loader;

                    SavedState.Default.KnownCollections.Add(saveDlg.FileName);
                }
            }
            var metadata = ModelConverter.ToStorage(Vm);
            Vm.Loader.UpdateMetadata(metadata);
            Vm.UnsavedChangesIndicatorVisibility = Visibility.Collapsed;
        }

        private Window GetParentWindow()
        {
            return Window.GetWindow(this);
        }

        private void Toggle_Click(object sender, RoutedEventArgs e)
        {
            Vm.IsExpanded = !Vm.IsExpanded;
        }

        private void SaveCollection_Click(object sender, RoutedEventArgs e)
        {
            SaveCollection();
        }

        private void EditCollection_Click(object sender, RoutedEventArgs e)
        {
            var result = CollectionEditorWindow.OpenModal(GetParentWindow(), Vm.Clone());
            if (result != null)
            {
                DataContext = result;
                result.UnsavedChangesIndicatorVisibility = Visibility.Visible;
            }
        }

        private void NewRequest_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
