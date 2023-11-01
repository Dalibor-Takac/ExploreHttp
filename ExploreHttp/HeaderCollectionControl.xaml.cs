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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExploreHttp
{
    /// <summary>
    /// Interaction logic for HeaderCollectionControl.xaml
    /// </summary>
    public partial class HeaderCollectionControl : UserControl
    {
        public HeaderCollectionControl()
        {
            InitializeComponent();
        }

        private static readonly DependencyProperty IsChechedPhantomHeaderProperty = DependencyProperty.Register(
            nameof(IsCheckedPhantomHeader),
            typeof(bool),
            typeof(HeaderCollectionControl),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));
        private static readonly DependencyProperty NamePhantomHeaderProperty = DependencyProperty.Register(
            nameof(NamePhantomHeader),
            typeof(string),
            typeof(HeaderCollectionControl),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));
        private static readonly DependencyProperty ValuePhantomHeaderProperty = DependencyProperty.Register(
            nameof(ValuePhantomHeader),
            typeof(string),
            typeof(HeaderCollectionControl),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));
        private static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
            nameof(Label),
            typeof(string),
            typeof(HeaderCollectionControl),
            new FrameworkPropertyMetadata("Headers", FrameworkPropertyMetadataOptions.Journal));

        public bool IsCheckedPhantomHeader
        {
            get => (bool)GetValue(IsChechedPhantomHeaderProperty);
            set => SetValue(IsChechedPhantomHeaderProperty, value);
        }
        public string NamePhantomHeader
        {
            get => (string)GetValue(NamePhantomHeaderProperty);
            set => SetValue(NamePhantomHeaderProperty, value);
        }
        public string ValuePhantomHeader
        {
            get => (string)GetValue(ValuePhantomHeaderProperty);
            set => SetValue(ValuePhantomHeaderProperty, value);
        }
        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public HeaderCollection Vm => (HeaderCollection)DataContext;

        private void OnAddHeader(object sender, RoutedEventArgs e)
        {
            Vm.Headers.Add(new HeaderItemModel(string.Empty, string.Empty));
        }
    }
}
