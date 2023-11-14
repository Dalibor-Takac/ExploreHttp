using ExploreHttp.Models;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Net.Http;

namespace ExploreHttp.Services;
public class TokenHelper
{
    private class TokenEntry
    {
        public string Token { get; set; }
        public DateTimeOffset ExpiresAt { get; set; }
    }

    private readonly ObjectPool<HttpClient> _clientPool;
    private readonly ConcurrentDictionary<string, TokenEntry> _cache;

    public TokenHelper(ObjectPool<HttpClient> clientPool)
    {
        _clientPool = clientPool;
        _cache = new();
    }

    private class TokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }

    public async Task<string> GetToken(Oauth2AuthenticationModel model)
    {
        var cacheKey = GetCacheKey(model);
        if (_cache.TryGetValue(cacheKey, out var cached))
        {
            if (DateTimeOffset.UtcNow < cached.ExpiresAt)
                return cached.Token;
        }

        using var tokenClient = _clientPool.LeaseItem();

        var response = await tokenClient.Object.PostAsync(model.AuthUrl, new FormUrlEncodedContent(GetFormData(model)));
        response.EnsureSuccessStatusCode();

        var buffer = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(buffer);

        _cache[cacheKey] = new TokenEntry() { Token = tokenResponse.AccessToken, ExpiresAt = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(tokenResponse.ExpiresIn) };

        return tokenResponse.AccessToken;
    }

    private string GetCacheKey(Oauth2AuthenticationModel model)
    {
        return $"{model.AuthUrl}:{model.ClientId}:{model.ClientSecret}:{model.Username}:{model.Password}:{model.Scope}:{model.Audience}:{model.GrantType}";
    }

    private IEnumerable<KeyValuePair<string, string>> GetFormData(Oauth2AuthenticationModel model)
    {
        yield return new KeyValuePair<string, string>("grant_Type", ConvertGrantType(model.GrantType));
        yield return new KeyValuePair<string, string>("client_id", model.ClientId);
        yield return new KeyValuePair<string, string>("client_secret", model.ClientSecret);
        if (model.GrantType == Oauth2GrantType.Password)
        {
            yield return new KeyValuePair<string, string>("username", model.Username);
            yield return new KeyValuePair<string, string>("password", model.Password);
        }
        yield return new KeyValuePair<string, string>("scope", model.Scope);
        yield return new KeyValuePair<string, string>("audience", model.Audience);
    }

    private string ConvertGrantType(Oauth2GrantType grantType)
    {
        return grantType switch
        {
            Oauth2GrantType.ClientCredentials => "client_credentials",
            Oauth2GrantType.Password => "password",
            _ => throw new InvalidOperationException($"Could not interpret value for supplied grant type of {grantType}")
        };
    }
}
