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

    private static DependencyProperty ApiLocationProperty = DependencyProperty.Register(
        nameof(ApiLocation),
        typeof(string),
        typeof(ImportOpenApiWindow),
        new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public string ApiLocation
    {
        get => GetValue(ApiLocationProperty) as string;
        set => SetValue(ApiLocationProperty, value);
    }

    private static DependencyProperty EndpointsProperty = DependencyProperty.Register(
        nameof(Endpoints),
        typeof(ObservableCollection<SavedRequest>),
        typeof(ImportOpenApiWindow),
        new FrameworkPropertyMetadata(new ObservableCollection<SavedRequest>(), FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public ObservableCollection<SavedRequest> Endpoints
    {
        get => GetValue(EndpointsProperty) as ObservableCollection<SavedRequest>;
        set => SetValue(EndpointsProperty, value);
    }

    private void OpenLocalFile_Click(object sender, RoutedEventArgs e)
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
            ApiLocation = dlg.FileName;
        }
    }

    private async void RunImportPreview_Click(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(ApiLocation))
        {
            var importer = new OpenApiImporter(ApiLocation, AppSettings);
            await importer.ImportPreview(Endpoints);
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

    public static async Task<RequestCollection> OpenDialog(Window parent, AppSettings appSettings)
    {
        var dlg = new ImportOpenApiWindow();
        dlg.Owner = parent;
        dlg.AppSettings = appSettings;
        
        if (dlg.ShowDialog().GetValueOrDefault() && !string.IsNullOrEmpty(dlg.ApiLocation))
        {
            var saveDlg = new SaveFileDialog();
            saveDlg.CheckPathExists = true;
            saveDlg.AddExtension = true;
            saveDlg.DefaultExt = "*.reqcol";
            saveDlg.Filter = "Request Collection|*.reqcol";

            if (saveDlg.ShowDialog(parent).GetValueOrDefault())
            {
                var importer = new OpenApiImporter(dlg.ApiLocation, appSettings);
                var result = await importer.ImportAndSave(saveDlg.FileName);
                return result;
            }
        }

        return null;
    }
}
