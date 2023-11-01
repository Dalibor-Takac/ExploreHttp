using ExploreHttp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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

        public static RequestCollection OpenModal(Window parent, RequestCollection editingInstance = default)
        {
            var vm = editingInstance ?? new RequestCollection();
            var dlg = new CollectionEditorWindow();
            dlg.Owner = parent;
            dlg.DataContext = vm;
            if (editingInstance != null)
                dlg.Title = "Explore HTTP - Edit Collection";

            if (dlg.ShowDialog().GetValueOrDefault())
                return vm;

            return null;
        }
    }
}
