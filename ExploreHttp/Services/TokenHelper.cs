using ExploreHttp.Models;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Net.Http;

namespace ExploreHttp.Services;
public class TokenHelper
{
    private readonly string _authUrl;

    private class TokenEntry
    {
        public string Token { get; set; }
        public DateTimeOffset ExpiresAt { get; set; }
    }

    private static readonly ConcurrentDictionary<string, TokenEntry> Cache = new();

    public TokenHelper(string authUrl)
    {
        _authUrl = authUrl;
    }

    private class TokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }

    public async Task<string> GetToken(Oauth2GrantType grantType, string clientId, string clientSecret, string username, string password, string scope, string audience)
    {
        var cacheKey = GetCacheKey(_authUrl, clientId, clientSecret, username, password, scope, audience, grantType);
        if (Cache.TryGetValue(cacheKey, out var cached))
        {
            if (DateTimeOffset.UtcNow < cached.ExpiresAt)
                return cached.Token;
        }

        using var tokenClient = new HttpClient();

        var response = await tokenClient.PostAsync(_authUrl, new FormUrlEncodedContent(GetFormData(grantType, clientId, clientSecret, username, password, scope, audience)));
        response.EnsureSuccessStatusCode();

        var buffer = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(buffer);

        Cache[cacheKey] = new TokenEntry() { Token = tokenResponse.AccessToken, ExpiresAt = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(tokenResponse.ExpiresIn) };

        return tokenResponse.AccessToken;
    }

    private string GetCacheKey(string authUrl, string clientId, string clientSecret, string username, string password, string scope, string audience, Oauth2GrantType grantType)
    {
        return $"{authUrl}:{clientId}:{clientSecret}:{username}:{password}:{scope}:{audience}:{grantType}";
    }

    private IEnumerable<KeyValuePair<string, string>> GetFormData(Oauth2GrantType grantType, string clientId, string clientSecret, string username, string password, string scope, string audience)
    {
        yield return new KeyValuePair<string, string>("grant_Type", ConvertGrantType(grantType));
        yield return new KeyValuePair<string, string>("client_id", clientId);
        yield return new KeyValuePair<string, string>("client_secret", clientSecret);
        if (grantType == Oauth2GrantType.Password)
        {
            yield return new KeyValuePair<string, string>("username", username);
            yield return new KeyValuePair<string, string>("password", password);
        }
        yield return new KeyValuePair<string, string>("scope", scope);
        yield return new KeyValuePair<string, string>("audience", audience);
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
