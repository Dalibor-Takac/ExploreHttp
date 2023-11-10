using ExploreHttp.Models;
using Newtonsoft.Json;

namespace ExploreHttp.Services.PersistanceModels;
public class EndpointCollection
{
    public string Name { get; set; }
    public CollectionKind Kind { get; set; }
    public string Source { get; set; }
    public List<RequestInfo> Requests { get; set; }
    public List<CollectionEnvironment> Environments { get; set; }
    public int SelectedEnvironmentIndex { get; set; }
    public bool IsExpanded { get; set; }
}

public class RequestInfo
{
    public Guid Id { get; set; }
    public RequestMethod Method { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public string OperationId { get; set; }
}

public class CollectionEnvironment
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<Variable> Variables { get; set; }
}

public class Variable
{
    public bool IsEnabled { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
}
