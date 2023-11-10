using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace ExploreHttp.Models;
public partial class OpenApiImportResult : ObservableObject
{
    private string documentLocation;
    private ObservableCollection<SelectableSavedRequest> endpoints;
    private OpenApiImportAction importOptions;
    private RequestCollection originalCollection;

    public string DocumentLocation { get => documentLocation; set => SetProperty(ref documentLocation,  value); }
    public ObservableCollection<SelectableSavedRequest> Endpoints { get => endpoints; set => SetProperty(ref endpoints,  value); }
    public OpenApiImportAction ImportOptions { get => importOptions; set => SetProperty(ref importOptions,  value); }
    public RequestCollection OriginalCollection { get => originalCollection; set => SetProperty(ref originalCollection, value); }
}
