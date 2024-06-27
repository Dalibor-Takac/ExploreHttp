using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace ExploreHttp.Models;
public partial class TreeNodeModel : ObservableObject
{
    private string nodeName;
    private string nodeValue;
    private ObservableCollection<TreeNodeModel> subNodes;

    public TreeNodeModel()
    {
        nodeName = string.Empty;
        nodeValue = string.Empty;
        subNodes = new ObservableCollection<TreeNodeModel>();
    }

    public string NodeName { get => nodeName; set => SetProperty(ref nodeName, value); }
    public string NodeValue { get => nodeValue; set => SetProperty(ref nodeValue, value); }
    public ObservableCollection<TreeNodeModel> SubNodes { get => subNodes; set => SetProperty(ref subNodes, value); }
}
