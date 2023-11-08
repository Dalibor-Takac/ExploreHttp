using ExploreHttp.Models;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ExploreHttp.Services;
public static class HttpHeadersExtensions
{
    public static IEnumerable<LogProperty> ToLogProperties(this HttpHeaders headers)
    {
        return headers.Select(x => new LogProperty()
        {
            PropertyName = x.Key,
            PropertyValue = string.Join(";", x.Value)
        });
    }

    public static IEnumerable<LogProperty> HeadersToLogProperties(this HttpRequestMessage request)
    {
        var result = request.Headers.ToLogProperties();
        if (request.Content is not null)
            result = result.Union(request.Content.Headers.ToLogProperties());
        return result;
    }

    public static IEnumerable<LogProperty> HeadersToLogProperties(this HttpResponseMessage response)
    {
        var result = response.Headers.ToLogProperties();
        if (response.Content is not null)
            result = result.Union(response.Content.Headers.ToLogProperties());
        return result;
    }
}
