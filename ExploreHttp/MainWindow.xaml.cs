using ExploreHttp.Models;
using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        }

        private void CloseRequest_Click(object sender, RoutedEventArgs e)
        {
            var request = (sender as FrameworkElement).DataContext as RequestModel;
            Vm.OpenRequests.Remove(request);
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OpenCollection_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "Request Collection|*.reqcol|All Files|*.*";
            dlg.CheckFileExists = true;
            dlg.CheckPathExists = true;
            if (dlg.ShowDialog(this).GetValueOrDefault())
            {
                //TODO open json file and add it to known collections
            }
        }
    }
}
