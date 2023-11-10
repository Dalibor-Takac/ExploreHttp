using ExploreHttp.Models;
using ExploreHttp.Services;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;

namespace ExploreHttp;
/// <summary>
/// Interaction logic for ImportOpenApiWindow.xaml
/// </summary>
public partial class ImportOpenApiWindow : Window
{
    public AppSettings AppSettings { get; set; }
    public ImportOpenApiWindow()
    {
        InitializeComponent();
    }

    OpenApiImportResult Vm => (OpenApiImportResult)DataContext;

    private async void OpenLocalFile_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog();
        dlg.CheckFileExists = true;
        dlg.CheckPathExists = true;
        dlg.Filter = "OpenAPI specs (JSON)|*.json|OpenAPI specs (YAML)|*.yml;*.yaml|All Files|*.*";
        dlg.FilterIndex = 1;
        dlg.Multiselect = false;
        dlg.Title = "Open local OpenAPI spec file";
        if (dlg.ShowDialog(this).GetValueOrDefault())
        {
            Vm.DocumentLocation = dlg.FileName;
            if (!string.IsNullOrEmpty(Vm.DocumentLocation))
            {
                try
                {
                    var importer = new OpenApiImporter(Vm.DocumentLocation, AppSettings);
                    await importer.ImportPreview(Vm.Endpoints);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    private async void RunImportPreview_Click(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(Vm.DocumentLocation))
        {
            try
            {
                var importer = new OpenApiImporter(Vm.DocumentLocation, AppSettings);
                await importer.ImportPreview(Vm.Endpoints);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    public static async Task<RequestCollection> OpenDialog(Window parent, AppSettings appSettings, RequestCollection collectionToOverride = default)
    {
        var dlg = new ImportOpenApiWindow();
        dlg.Owner = parent;
        dlg.AppSettings = appSettings;
        dlg.DataContext = new OpenApiImportResult()
        {
            Endpoints = new ObservableCollection<SelectableSavedRequest>(),
            DocumentLocation = collectionToOverride?.Source,
            OriginalCollection = collectionToOverride,
            ImportOptions = OpenApiImportAction.All
        };
        
        if (dlg.ShowDialog().GetValueOrDefault() && !string.IsNullOrEmpty(dlg.Vm.DocumentLocation))
        {
            var saveDlg = new SaveFileDialog();
            saveDlg.CheckPathExists = true;
            saveDlg.AddExtension = true;
            saveDlg.DefaultExt = "*.reqcol";
            saveDlg.Filter = "Request Collection|*.reqcol";

            if (dlg.Vm.OriginalCollection != null || saveDlg.ShowDialog(parent).GetValueOrDefault())
            {
                try
                {
                    var importer = new OpenApiImporter(dlg.Vm.DocumentLocation, appSettings);
                    if (dlg.Vm.OriginalCollection is not null)
                    {
                        var selectedOperationIds = dlg.Vm.Endpoints.Where(x => x.Selected).Select(x => x.OperationId);
                        await importer.RefreshImport(dlg.Vm.OriginalCollection, dlg.Vm.ImportOptions, selectedOperationIds);
                        return dlg.Vm.OriginalCollection;
                    }
                    else
                    {
                        var result = await importer.ImportAndSave(saveDlg.FileName);
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        return null;
    }
}
