using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace ExploreHttp.Models;
public partial class QueryStringModel : ObservableObject
{
    private string renderedQueryString;
    private ObservableCollection<QueryStringParameter> parameters;

    public string RenderedQueryString { get => renderedQueryString; set => SetProperty(ref renderedQueryString, value); }
    public ObservableCollection<QueryStringParameter> Parameters { get => parameters; set => SetProperty(ref parameters, value); }

    public QueryStringModel()
    {
        Parameters = new ObservableCollection<QueryStringParameter>();
        Parameters.CollectionChanged += (sender, e) =>
        {
            RenderedQueryString = RenderQueryString();
        };
    }

    public string RenderQueryString()
    {
        var sb = new StringBuilder();
        var isFirst = true;
        foreach (var item in Parameters.Where(x => x.IsEnabled && !string.IsNullOrEmpty(x.ParameterName)))
        {
            if (isFirst)
                isFirst = false;
            else
                sb.Append("&");

            sb.Append($"{item.ParameterName}={item.ParameterValue}");
        }

        return sb.ToString();
    }
}

public partial class QueryStringParameter : ObservableObject
{
    private bool isEnabled;
    private string parameterName;
    private string parameterValue;

    public bool IsEnabled { get => isEnabled; set => SetProperty(ref isEnabled, value); }
    public string ParameterName { get => parameterName; set => SetProperty(ref parameterName, value); }
    public string ParameterValue { get => parameterValue; set => SetProperty(ref parameterValue, value); }
}
