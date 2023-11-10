namespace ExploreHttp.Models;
public class SelectableSavedRequest : SavedRequest
{
    private bool selected;
    public bool Selected { get => selected; set => SetProperty(ref selected, value); }
    public SelectableSavedRequest(RequestCollection parentCollection) : base(parentCollection)
    { }
}
