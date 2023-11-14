using CommunityToolkit.Mvvm.ComponentModel;
using DotLiquid;
using DotLiquid.Exceptions;
using ExploreHttp.Services;
using System.Collections.ObjectModel;

namespace ExploreHttp.Models;
public partial class QueryStringModel : ObservableObject
{
    private readonly RequestModel _parentRequest;
    private string renderedQueryString;
    private ObservableCollection<QueryStringParameter> parameters;

    public string RenderedQueryString { get => renderedQueryString; set => SetProperty(ref renderedQueryString, value); }
    public ObservableCollection<QueryStringParameter> Parameters { get => parameters; set => SetProperty(ref parameters, value); }

    public QueryStringModel(RequestModel requestModel)
    {
        _parentRequest = requestModel;
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

        var locals = RequestRunner.EnvironmentToLocals(_parentRequest.SavedRequest.ParentCollection);
        foreach (var item in Parameters.Where(x => x.IsEnabled && !string.IsNullOrEmpty(x.ParameterName)))
        {
            if (isFirst)
                isFirst = false;
            else
                sb.Append("&");
            try
            {
                var nameTemplate = Template.Parse(item.ParameterName);
                var valueTemplate = Template.Parse(item.ParameterValue);

                sb.Append($"{Uri.EscapeDataString(nameTemplate.Render(locals))}={Uri.EscapeDataString(valueTemplate.Render(locals))}");
            }
            catch (SyntaxException)
            {
                sb.Append($"{Uri.EscapeDataString(item.ParameterName)}={Uri.EscapeDataString(item.ParameterValue)}");
            }
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
