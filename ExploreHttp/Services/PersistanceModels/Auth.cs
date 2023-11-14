using ExploreHttp.Models;

namespace ExploreHttp.Services.PersistanceModels;
public class BasicAuth
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class BearerAuth
{
    public string Scheme { get; set; }
    public string Parameter { get; set; }
}

public class Oauth2Auth
{
    public string AuthUrl { get; set; }
    public Oauth2GrantType GrantType { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Scope { get; set; }
    public string Audience { get; set; }
}
