using ExploreHttp.Services.PersistanceModels;
using Newtonsoft.Json;
using System.IO;
using System.IO.Compression;

namespace ExploreHttp.Services;
public class CollectionLoader
{
    private readonly JsonSerializer _serializer;
    private readonly string _fileName;

    private const string COLLECTION_METADATA_FILE = "metadata.json";
    private const string REQUESTS_FILE_FORMAT = "requests\\{0}.request.json";// 0: request id

    public CollectionLoader(string filename)
    {
        _fileName = filename;
        _serializer = new JsonSerializer();
    }

    private static ZipArchive NewOrOpenForUpdate(string filename)
    {
        if (File.Exists(filename))
            return ZipFile.Open(filename, ZipArchiveMode.Update);
        else
        {
            using (var zip = ZipFile.Open(filename, ZipArchiveMode.Create))
            {
                //nothing to do here, just to create file contents and let it be disposed saving it in the process
            }

            return ZipFile.Open(filename, ZipArchiveMode.Update);
        }
    }

    public EndpointCollection ReadMetadata()
    {
        using var localFile = NewOrOpenForUpdate(_fileName);

        var archiveEntry = localFile.GetEntry(COLLECTION_METADATA_FILE) ?? throw new InvalidOperationException("No collection metadata found in request colection");
        using var stream = archiveEntry.Open();
        using var reader = new StreamReader(stream);
        using var jsonReader = new JsonTextReader(reader);
        var result = _serializer.Deserialize<EndpointCollection>(jsonReader);
        return result;
    }

    public void UpdateMetadata(EndpointCollection data)
    {
        using var localFile = NewOrOpenForUpdate(_fileName);

        var archiveEntry = localFile.Entries.FirstOrDefault(x => x.FullName == COLLECTION_METADATA_FILE);
        if (archiveEntry is null)
            archiveEntry = localFile.CreateEntry(COLLECTION_METADATA_FILE, CompressionLevel.Optimal);

        using var stream = archiveEntry.Open();
        using var writter = new StreamWriter(stream);
        using var jsonWritter = new JsonTextWriter(writter);
        _serializer.Serialize(jsonWritter, data);
    }

    public Request LoadRequest(Guid requestId)
    {
        using var localFile = NewOrOpenForUpdate(_fileName);
        var archiveEntry = localFile.GetEntry(string.Format(REQUESTS_FILE_FORMAT, requestId));
        if (archiveEntry is null)
            return null;

        using var stream = archiveEntry.Open();
        using var reader = new StreamReader(stream);
        using var jsonReader = new JsonTextReader(reader);
        var result = _serializer.Deserialize<Request>(jsonReader);
        result.Id = requestId;
        return result;
    }

    public void SaveRequest(Request request)
    {
        using var localFile = NewOrOpenForUpdate(_fileName);

        var entryFullName = string.Format(REQUESTS_FILE_FORMAT, request.Id);
        var archiveEntry = localFile.Entries.FirstOrDefault(x => x.FullName == entryFullName);
        if (archiveEntry is null)
            archiveEntry = localFile.CreateEntry(entryFullName, CompressionLevel.Optimal);

        using var stream = archiveEntry.Open();
        using var writter = new StreamWriter(stream);
        using var jsonWritter = new JsonTextWriter(writter);
        _serializer.Serialize(jsonWritter, request);
    }

    public void RemoveRequest(Guid requestId)
    {
        var metadata = ReadMetadata();
        var toRemove = metadata.Requests.FirstOrDefault(x => x.Id == requestId) ?? throw new InvalidOperationException("Request with given id does not exist in metadata, nothing to do");
        metadata.Requests.Remove(toRemove);
        UpdateMetadata(metadata);

        using var localFile = NewOrOpenForUpdate(_fileName);

        var entryFullName = string.Format(REQUESTS_FILE_FORMAT, requestId);
        var archiveEntry = localFile.Entries.FirstOrDefault(x => x.FullName == entryFullName);
        if (archiveEntry is not null)
        {
            archiveEntry.Delete();
        }
    }

    public string FileName => _fileName;
}
