using System.Windows;
using System.Windows.Controls;

namespace ExploreHttp
{
    /// <summary>
    /// Interaction logic for TextBoxWithPlaceholder.xaml
    /// </summary>
    public partial class TextBoxWithPlaceholder : UserControl
    {
        public TextBoxWithPlaceholder()
        {
            InitializeComponent();
        }

        private static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(TextBoxWithPlaceholder),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));
        private static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register(
            nameof(Placeholder),
            typeof(string),
            typeof(TextBoxWithPlaceholder),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.Journal));
        private static readonly RoutedEvent TextPropertyChangedEvent = EventManager.RegisterRoutedEvent(
            nameof(TextChanged),
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(TextBoxWithPlaceholder));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set
            {
                RaiseEvent(new RoutedEventArgs(TextPropertyChangedEvent));
                SetValue(TextProperty, value);
            }
        }

        public event RoutedEventHandler TextChanged
        {
            add => AddHandler(TextPropertyChangedEvent, value);
            remove => RemoveHandler(TextPropertyChangedEvent, value);
        }

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }
    }
}
