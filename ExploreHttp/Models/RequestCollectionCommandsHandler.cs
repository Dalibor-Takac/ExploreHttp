using ExploreHttp.Services;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace ExploreHttp.Models;
public class RequestCollectionCommandsHandler
{
    private readonly Window _hostWindow;
    private readonly ApplicationViewModel _vm;

    public RequestCollectionCommandsHandler(Window hostWindow, ApplicationViewModel vm)
    {
        _hostWindow = hostWindow;
        _vm = vm;
    }

    public void NewCollectionCommandHandler(object sender, ExecutedRoutedEventArgs e)
    {
        var newCollection = CollectionEditorWindow.OpenModal(_hostWindow);
        if (newCollection != null)
            _vm.Collections.Add(newCollection);
    }

    public void OpenCollectionCommandHandler(object sender, ExecutedRoutedEventArgs e)
    {
        var dlg = new OpenFileDialog();
        dlg.Filter = "Request Collection|*.reqcol|All Files|*.*";
        dlg.CheckFileExists = true;
        dlg.CheckPathExists = true;
        if (dlg.ShowDialog(_hostWindow).GetValueOrDefault())
        {
            if (File.Exists(dlg.FileName) && !SavedState.Default.KnownCollections.Contains(dlg.FileName))
            {
                SavedState.Default.KnownCollections.Add(dlg.FileName);
                SavedState.Default.Save();

                var loader = new CollectionLoader(dlg.FileName);

                var metadata = loader.ReadMetadata();
                var requestCollection = ModelConverter.FromStorage(metadata);
                requestCollection.Loader = loader;

                _vm.Collections.Add(requestCollection);
            }
        }
    }

    public void SaveCollectionCommandHandler(object sender, ExecutedRoutedEventArgs e)
    {
        var collection = e.Parameter as RequestCollection;
        if (collection.Loader is null)
        {
            var saveDlg = new SaveFileDialog();
            saveDlg.CheckPathExists = true;
            saveDlg.AddExtension = true;
            saveDlg.DefaultExt = "*.reqcol";
            saveDlg.Filter = "Request Collection|*.reqcol";
            if (saveDlg.ShowDialog().GetValueOrDefault() && !SavedState.Default.KnownCollections.Contains(saveDlg.FileName))
            {
                var loader = new CollectionLoader(saveDlg.FileName);
                collection.Loader = loader;

                SavedState.Default.KnownCollections.Add(saveDlg.FileName);
            }
        }
        var metadata = ModelConverter.ToStorage(collection);
        collection.Loader.UpdateMetadata(metadata);
        collection.UnsavedChangesIndicatorVisibility = Visibility.Collapsed;
    }

    public void EditCollectionCommandHandler(object sender, ExecutedRoutedEventArgs e)
    {
        var collection = e.Parameter as RequestCollection;
        var result = CollectionEditorWindow.OpenModal(_hostWindow, collection.Clone());
        if (result != null)
        {
            collection.SyncWithOther(result);
            collection.UnsavedChangesIndicatorVisibility = Visibility.Visible;
        }
    }

    public void CloseCollectionCommandHandler(object sender, ExecutedRoutedEventArgs e)
    {
        var collection = e.Parameter as RequestCollection;
        if (collection.UnsavedChangesIndicatorVisibility == Visibility.Visible)
        {
            if (MessageBox.Show(_hostWindow,
                "Are you sure you want to close collection with unsaved changes?",
                "There are unsaved changes!",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Exclamation) == MessageBoxResult.Cancel)
            {
                return;
            }

            _vm.Collections.Remove(collection);
        }
    }

    public void BindAllCommands(CommandBindingCollection bindings)
    {
        bindings.Add(new CommandBinding(Application.Current.FindResource(CommandNames.NewCollectionCommandName) as ICommand, NewCollectionCommandHandler));
        bindings.Add(new CommandBinding(Application.Current.FindResource(CommandNames.OpenCollectionCommandName) as ICommand, OpenCollectionCommandHandler));
        bindings.Add(new CommandBinding(Application.Current.FindResource(CommandNames.EditCollectionCommandName) as ICommand, EditCollectionCommandHandler));
        bindings.Add(new CommandBinding(Application.Current.FindResource(CommandNames.SaveCollectionCommandName) as ICommand, SaveCollectionCommandHandler));
        bindings.Add(new CommandBinding(Application.Current.FindResource(CommandNames.CloseCollectionCommandName) as ICommand, CloseCollectionCommandHandler));
    }
}
