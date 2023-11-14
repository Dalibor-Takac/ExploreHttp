using CommunityToolkit.Mvvm.ComponentModel;
using ExploreHttp.Services;
using System.Net.Http;

namespace ExploreHttp.Models;
public abstract class AuthenticationModel : ObservableObject
{
    public abstract void AddAuthentication(HttpRequestMessage request);
}

public partial class BasicAuthenticationModel : AuthenticationModel
{
    private string username;
    private string password;

    public string Username { get => username; set => SetProperty(ref username,  value); }
    public string Password { get => password; set => SetProperty(ref password, value); }

    public override void AddAuthentication(HttpRequestMessage request)
    {
        var parm = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Uri.EscapeDataString(username)}:{Uri.EscapeDataString(password)}"));
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", parm);
    }
}

public partial class BearerAuthenticationModel : AuthenticationModel
{
    private string scheme;
    private string parameter;

    public string Scheme { get => scheme; set => SetProperty(ref scheme, value); }
    public string Parameter { get => parameter; set => SetProperty(ref parameter, value); }

    public override void AddAuthentication(HttpRequestMessage request)
    {
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(scheme, parameter);
    }
}

public partial class Oauth2AuthenticationModel : AuthenticationModel
{
    private string authUrl;
    private string scope;
    private string audience;
    private Oauth2GrantType grantType;
    private string clientId;
    private string clientSecret;
    private string username;
    private string password;

    public string AuthUrl { get => authUrl; set => SetProperty(ref authUrl, value); }
    public string Scope { get => scope; set => SetProperty(ref scope, value); }
    public string Audience { get => audience; set => SetProperty(ref audience, value); }
    public Oauth2GrantType GrantType { get => grantType; set => SetProperty(ref grantType, value); }
    public string ClientId { get => clientId; set => SetProperty(ref clientId, value); }
    public string ClientSecret { get => clientSecret; set => SetProperty(ref clientSecret, value); }
    public string Username { get => username; set => SetProperty(ref username, value); }
    public string Password { get => password; set => SetProperty(ref password, value); }

    public override void AddAuthentication(HttpRequestMessage request)
    {
        var tokenHelper = new TokenHelper(AuthUrl);
        var token = tokenHelper.GetToken(GrantType, ClientId, ClientSecret, Username, Password, Scope, Audience);
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }
}

public enum Oauth2GrantType
{
    ClientCredentials,
    Password,
}
