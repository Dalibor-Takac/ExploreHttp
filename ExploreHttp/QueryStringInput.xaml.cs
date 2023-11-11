using ExploreHttp.Models;
using System.Windows;
using System.Windows.Controls;

namespace ExploreHttp;
/// <summary>
/// Interaction logic for QueryStringInput.xaml
/// </summary>
public partial class QueryStringInput : UserControl
{
    public QueryStringInput()
    {
        InitializeComponent();
    }

    QueryStringModel Vm => (QueryStringModel) DataContext;

    private void AddParameter_Click(object sender, RoutedEventArgs e)
    {
        var parm = new QueryStringParameter()
        {
            IsEnabled = true
        };
        parm.PropertyChanged += (sender, e) => { Vm.RenderedQueryString = Vm.RenderQueryString(); };
        Vm.Parameters.Add(parm);
    }
}
