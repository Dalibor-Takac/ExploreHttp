using ExploreHttp.Models;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;

namespace ExploreHttp.Services;
public class OpenApiImporter
{
    private readonly string _specLocation;
    private readonly AppSettings _appSettings;

    private static readonly IEnumerable<OperationType> NoBodyOperations = new[]
    {
        OperationType.Get,
        OperationType.Head,
        OperationType.Options
    };

    public OpenApiImporter(string specLocation, AppSettings appSettings)
    {
        _specLocation = specLocation;
        _appSettings = appSettings;
    }

    private IEnumerable<(SavedRequest savedRequest, RequestModel requestModel)> ImportEndpoints(OpenApiDocument document, RequestCollection result)
    {
        foreach (var path in document.Paths)
        {
            foreach (var operation in path.Value.Operations)
            {
                var savedRequest = new SavedRequest(result)
                {
                    Method = operation.Key.ToRequestMethod(),
                    Name = operation.Value.Summary ?? operation.Value.OperationId ?? $"[{operation.Key}]{path.Key}",
                    Url = "{{baseAddress}}" + path.Key,
                    OperationId = operation.Value.OperationId ?? $"[{operation.Key}]{path.Key}"
                };
                var requestModel = new RequestModel(savedRequest);
                if (operation.Value.Parameters is not null && operation.Value.Parameters.Count > 0)
                {
                    foreach (var parm in operation.Value.Parameters)
                    {
                        if (parm.In == ParameterLocation.Query)
                        {
                            var qparam = new QueryStringParameter()
                            {
                                IsEnabled = parm.Required,
                                ParameterName = parm.Name,
                                ParameterValue = parm.Schema.Format ?? parm.Schema.Type
                            };
                            qparam.PropertyChanged += (sender, e) => { requestModel.QueryString.RenderedQueryString = requestModel.QueryString.RenderQueryString(); };
                            requestModel.QueryString.Parameters.Add(qparam);
                        }
                        else if (parm.In == ParameterLocation.Header)
                            requestModel.RequestHeaders.Headers.Add(new HeaderItemModel()
                            {
                                IsEnabled = parm.Required,
                                HeaderName = parm.Name,
                                HeaderValue = parm.Schema.Format ?? parm.Schema.Type
                            });
                    }
                }
                var firstBodyContent = operation.Value.RequestBody?.Content.FirstOrDefault();
                if (firstBodyContent is not null)
                {
                    var schema = firstBodyContent.Value.Value.Schema.GetEffective(document);
                    requestModel.RequestHeaders.Headers.Add(new HeaderItemModel("Content-Type", firstBodyContent.Value.Key));
                    var bodyBuilder = new StringBuilder();
                    bodyBuilder.AppendLine("{");
                    bool isFirst = true;
                    foreach (var prop in schema.Properties)
                    {
                        if (isFirst)
                            isFirst = false;
                        else
                            bodyBuilder.AppendLine(",");
                        bodyBuilder.AppendFormat("\t\"{0}\": \"{1}\"", prop.Key, prop.Value.Format ?? prop.Value.Type);
                    }
                    bodyBuilder.AppendLine("\n}");
                    requestModel.RequestBody.Source = bodyBuilder.ToString();
                }
                requestModel.RequestHeaders.Headers.Add(new HeaderItemModel("User-Agent", _appSettings.UserAgentString));
                yield return (savedRequest, requestModel);
            }
        }
    }

    public async Task<RequestCollection> ImportAndSave(string fileName)
    {
        var document = await ImportDocument();
        var result = new RequestCollection()
        {
            CollectionName = document.Info.Title,
            Kind = CollectionKind.OpenApi,
            Source = _specLocation
        };

        var envIndex = 1;
        if (document.Servers is not null && document.Servers.Count > 0)
        {
            foreach (var host in document.Servers)
            {
                var env = new SavedEnvironment()
                {
                    Name = host.Description ?? $"env#{envIndex}"
                };
                env.Variables.Add(new EnvironmentVariable() { IsEnabled = true, Name = "baseAddress", Value = host.Url });
                foreach (var var in host.Variables)
                {
                    env.Variables.Add(new EnvironmentVariable() { IsEnabled = true, Name = var.Key, Value = var.Value.Default });
                }
                result.SavedEnvironments.Add(env);
                ++envIndex;
            }
        }
        else
        {
            var env = new SavedEnvironment()
            {
                Name = "Default"
            };
            env.Variables.Add(new EnvironmentVariable() { IsEnabled = false, Name = "baseAddress", Value = string.Empty });
            result.SavedEnvironments.Add(env);
        }
        result.SelectedEnvironmentIndex = 0;

        var importedEndpointResult = ImportEndpoints(document, result).ToArray();

        result.Loader = new CollectionLoader(fileName);
        foreach (var item in importedEndpointResult)
        {
            var storageRequest = ModelConverter.ToStorage(item.requestModel);
            result.Loader.SaveRequest(storageRequest);
            result.SavedRequests.Add(item.savedRequest);
        }
        var metadata = ModelConverter.ToStorage(result);
        result.Loader.UpdateMetadata(metadata);

        result.UnsavedChangesIndicatorVisibility = System.Windows.Visibility.Collapsed;

        return result;
    }

    private async Task<OpenApiDocument> ImportDocument()
    {
        if (_specLocation.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase)
            || _specLocation.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
        {
            using var client = new HttpClient();
            using var resultStream = await client.GetStreamAsync(_specLocation);
            var result = await new OpenApiStreamReader().ReadAsync(resultStream);
            return result.OpenApiDocument;
        }
        else
        {
            using var fileStream = new FileStream(_specLocation, FileMode.Open, FileAccess.Read);
            var result = await new OpenApiStreamReader().ReadAsync(fileStream);
            return result.OpenApiDocument;
        }
    }

    public async Task ImportPreview(ObservableCollection<SelectableSavedRequest> endpoints)
    {
        var document = await ImportDocument();
        endpoints.Clear();
        var baseUrl = document.Servers.FirstOrDefault()?.Url ?? string.Empty;
        foreach (var path in document.Paths)
        {
            foreach (var operation in path.Value.Operations)
            {
                endpoints.Add(new SelectableSavedRequest(null)
                {
                    Method = operation.Key.ToRequestMethod(),
                    Name = operation.Value.Summary ?? operation.Value.OperationId,
                    Url = $"{baseUrl}{path.Key}",
                    OperationId = operation.Value.OperationId,
                    Selected = true
                });
            }
        }
    }

    public async Task RefreshImport(RequestCollection collection,
                                    OpenApiImportAction importOptions,
                                    IEnumerable<string> selectedOperationIds)
    {
        var document = await ImportDocument();
        var importedEndpoints = ImportEndpoints(document, collection).ToArray();

        var filteredEndpointsToMerge = importedEndpoints.Where(x => ImportOptionsFilter(importOptions, x.savedRequest, collection.SavedRequests, selectedOperationIds));

        foreach (var item in filteredEndpointsToMerge)
        {
            var existingRequest = collection.SavedRequests.FirstOrDefault(x => x.OperationId == item.savedRequest.OperationId);
            if (existingRequest is not null)
            {
                // override existing, make sure ids are correct (take new from import)
                existingRequest.Url = item.savedRequest.Url;
                collection.Loader.RemoveRequest(existingRequest.Id);
                existingRequest.Id = item.savedRequest.Id;
                var toSave = ModelConverter.ToStorage(item.requestModel);
                collection.Loader.SaveRequest(toSave);
            }
            else
            {
                // add new request
                collection.SavedRequests.Add(item.savedRequest);
                var toSave = ModelConverter.ToStorage(item.requestModel);
                collection.Loader.SaveRequest(toSave);
            }
        }
        var metadata = ModelConverter.ToStorage(collection);
        collection.Loader.UpdateMetadata(metadata);

        collection.UnsavedChangesIndicatorVisibility = System.Windows.Visibility.Collapsed;
    }

    private bool ImportOptionsFilter(OpenApiImportAction importOptions,
                                     SavedRequest importedRequest,
                                     IEnumerable<SavedRequest> existingRequests,
                                     IEnumerable<string> selectedoperationIds)
    {
        return importOptions switch
        {
            OpenApiImportAction.All => true,
            OpenApiImportAction.New => !existingRequests.Any(x => x.OperationId == importedRequest.OperationId),
            OpenApiImportAction.Custom => selectedoperationIds.Any(x => x == importedRequest.OperationId),
            _ => false
        };
    }
}

public static class OpenApiExtensions
{
    public static RequestMethod ToRequestMethod(this OperationType operationType)
    {
        return operationType switch
        {
            OperationType.Get => RequestMethod.Get,
            OperationType.Post => RequestMethod.Post,
            OperationType.Put => RequestMethod.Put,
            OperationType.Delete => RequestMethod.Delete,
            OperationType.Options => RequestMethod.Options,
            OperationType.Head => RequestMethod.Head,
            OperationType.Patch => RequestMethod.Patch,
            _ => RequestMethod.Get
        };
    }
}
