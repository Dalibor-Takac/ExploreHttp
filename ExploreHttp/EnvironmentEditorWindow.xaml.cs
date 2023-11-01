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
    /// Interaction logic for EnvironmentEditorWindow.xaml
    /// </summary>
    public partial class EnvironmentEditorWindow : Window
    {
        public EnvironmentEditorWindow()
        {
            InitializeComponent();
        }

        private SavedEnvironment Vm => (SavedEnvironment)DataContext;

        public static SavedEnvironment OpenModal(Window parent, SavedEnvironment editingInstance = default)
        {
            var vm = editingInstance ?? new SavedEnvironment();

            var dlg = new EnvironmentEditorWindow();
            dlg.DataContext = vm;
            dlg.Owner = parent;
            if (editingInstance != null)
                dlg.Title = "Edit Environment";

            if (dlg.ShowDialog().GetValueOrDefault())
                return vm;

            return null;
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

        private void AddVariable_Click(object sender, RoutedEventArgs e)
        {
            Vm.Variables.Add(new EnvironmentVariable() { IsEnabled = true });
        }
    }
}
