using CommunityToolkit.Mvvm.ComponentModel;

namespace ExploreHttp.Models;
public partial class AuthenticationProvider : ObservableObject
{
    private AuthenticationKind kind;
    private Dictionary<string, string> authParameters;

    public AuthenticationKind Kind { get => kind; set => SetProperty(ref kind, value); }
    public Dictionary<string, string> AuthParameters { get => authParameters; set => SetProperty(ref authParameters,  value); }

    public AuthenticationProvider()
    {
        Kind = AuthenticationKind.None;
        AuthParameters = new Dictionary<string, string>
        {
            { AuthenticationParameterNames.Oauth2GrantType, "password" }
        };
    }
}

public enum AuthenticationKind
{
    None,
    Basic,
    Bearer,
    Oauth2,
}

public static class AuthenticationParameterNames
{
    public static string BasicUsername = "basic.username";
    public static string BasicPassword = "basic.password";

    public static string BearerScheme = "bearer.scheme";
    public static string BearerToken = "bearer.token";

    public static string Oauth2GrantType = "oauth2.grantType";
    public static string Oauth2ClientId = "oauth2.clientId";
    public static string Oauth2ClientSecret = "oauth2.clientSecret";
    public static string Oauth2Username = "oauth2.username";
    public static string Oauth2Password = "oauth2.password";
    public static string Oauth2Scope = "oauth2.scope";
    public static string Oauth2Audience = "oauth2.audience";
}
