using ExploreHttp.Models;
using System.ComponentModel;
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

    private void UpdateRenderedQueryString(object sender, PropertyChangedEventArgs e)
    {
        Vm.RenderedQueryString = Vm.RenderQueryString();
    }

    private void AddParameter_Click(object sender, RoutedEventArgs e)
    {
        var parm = new QueryStringParameter()
        {
            IsEnabled = true
        };
        parm.PropertyChanged += UpdateRenderedQueryString;
        Vm.Parameters.Add(parm);
    }

    private void DeleteParameter_Click(object sender, RoutedEventArgs e)
    {
        var parameter = (sender as FrameworkElement).DataContext as QueryStringParameter;
        if (parameter is not null)
        {
            parameter.PropertyChanged -= UpdateRenderedQueryString;
            Vm.Parameters.Remove(parameter);
        }
    }
}
