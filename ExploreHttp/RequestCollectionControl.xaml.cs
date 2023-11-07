using ExploreHttp.Models;
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

        private void Toggle_Click(object sender, RoutedEventArgs e)
        {
            Vm.IsExpanded = !Vm.IsExpanded;
        }
    }
}
