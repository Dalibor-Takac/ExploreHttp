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
                    Name = operation.Value.Summary ?? operation.Value.OperationId,
                    Url = "{{baseAddress}}" + path.Key
                };
                var requestModel = new RequestModel(savedRequest);
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
                        bodyBuilder.AppendFormat("\t\"{0}\": \"{1}\"", prop.Key, prop.Value.Type);
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

    public async Task ImportPreview(ObservableCollection<SavedRequest> endpoints)
    {
        var document = await ImportDocument();
        endpoints.Clear();
        foreach (var path in document.Paths)
        {
            foreach (var operation in path.Value.Operations)
            {
                endpoints.Add(new SavedRequest(null)
                {
                    Method = operation.Key.ToRequestMethod(),
                    Name = operation.Value.Summary ?? operation.Value.OperationId,
                    Url = $"{document.Servers.FirstOrDefault().Url}/{path.Key}"
                });
            }
        }
    }

    public Task RefreshImport(RequestCollection vm, string documentLocation, OpenApiImportAction importOptions)
    {
        //TODO figure out what to do to refresh existing endpoints, how to identify them, perhaps operation id?
        throw new NotImplementedException();
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
