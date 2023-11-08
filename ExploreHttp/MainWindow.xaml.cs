using ExploreHttp.Models;
using ExploreHttp.Services;
using Microsoft.Win32;
using System.Collections.Specialized;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace ExploreHttp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ApplicationViewModel Vm { get; }
        private readonly RequestCollectionCommandsHandler collectionCommandHandlers;
        private readonly RequestModelCommandsHandler requestCommandHandlers;
        public MainWindow()
        {
            if (SavedState.Default.KnownCollections is null)
                SavedState.Default.KnownCollections = new StringCollection();

            Vm = new ApplicationViewModel(SavedState.Default);

            collectionCommandHandlers = new RequestCollectionCommandsHandler(this, Vm);
            collectionCommandHandlers.BindAllCommands(CommandBindings);

            requestCommandHandlers = new RequestModelCommandsHandler(this, Vm, selectedTabIndex => mainTabs.SelectedIndex = selectedTabIndex);
            requestCommandHandlers.BindAllCommands(CommandBindings);
            
            CommandBindings.Add(new CommandBinding(Application.Current.FindResource(CommandNames.OpenSettingsCommandName) as ICommand, OpenSettingsCommandHandler));
            CommandBindings.Add(new CommandBinding(Application.Current.FindResource(CommandNames.AboutCommandName) as ICommand, AboutCommandHandler));

            DataContext = Vm;
            InitializeComponent();
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Vm.Collections.Any(x => x.UnsavedChangesIndicatorVisibility == Visibility.Visible)
                || Vm.OpenRequests.Any(x => x.UnsavedChangesIndicatorVisibility == Visibility.Visible))
            {
                if (MessageBox.Show(this,
                                    "Are you sure you want to quit with unsaved changes?",
                                    "There are unsaved changes!",
                                    MessageBoxButton.OKCancel,
                                    MessageBoxImage.Exclamation) == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }
            Vm.ToSettings(SavedState.Default);
            SavedState.Default.Save();
        }

        private void OpenSettingsCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            var newSettings = SettingsWindow.OpenModal(this, Vm.AppSettings.Clone());
            if (newSettings != null)
                Vm.AppSettings = newSettings;
        }


        private void AboutCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            AboutWindow.OpenDialog(this);
        }
    }
}
