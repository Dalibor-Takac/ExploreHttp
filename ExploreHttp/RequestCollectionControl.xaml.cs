using ExploreHttp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for RequestCollectionControl.xaml
    /// </summary>
    public partial class RequestCollectionControl : UserControl
    {
        public RequestCollectionControl()
        {
            InitializeComponent();
        }

        private RequestCollection Vm => (RequestCollection)DataContext;

        private void Toggle_Click(object sender, RoutedEventArgs e)
        {
            Vm.IsExpanded = !Vm.IsExpanded;
            Debug.WriteLine($"Toggle clicked, new expanded value is {Vm.IsExpanded}");
        }
    }
}
