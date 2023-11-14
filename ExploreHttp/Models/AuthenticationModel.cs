using CommunityToolkit.Mvvm.ComponentModel;

namespace ExploreHttp.Models;

public partial class BasicAuthenticationModel : ObservableObject
{
    private string username;
    private string password;

    public string Username { get => username; set => SetProperty(ref username,  value); }
    public string Password { get => password; set => SetProperty(ref password, value); }
}

public partial class BearerAuthenticationModel : ObservableObject
{
    private string scheme;
    private string parameter;

    public string Scheme { get => scheme; set => SetProperty(ref scheme, value); }
    public string Parameter { get => parameter; set => SetProperty(ref parameter, value); }
}

public partial class Oauth2AuthenticationModel : ObservableObject
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
}

public enum Oauth2GrantType
{
    ClientCredentials,
    Password,
}
