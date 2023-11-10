using CommunityToolkit.Mvvm.ComponentModel;

namespace ExploreHttp.Models;
public partial class OpenApiImportResult : ObservableObject
{
    private string documentLocation;
    private RequestCollection collection;
    private OpenApiImportAction importOptions;
    private RequestCollection originalCollection;

    public string DocumentLocation { get => documentLocation; set => SetProperty(ref documentLocation,  value); }
    public RequestCollection Collection { get => collection; set => SetProperty(ref collection,  value); }
    public OpenApiImportAction ImportOptions { get => importOptions; set => SetProperty(ref importOptions,  value); }
    public RequestCollection OriginalCollection { get => originalCollection; set => SetProperty(ref originalCollection, value); }
}
