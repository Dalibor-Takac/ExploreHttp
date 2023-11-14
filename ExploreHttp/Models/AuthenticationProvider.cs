using CommunityToolkit.Mvvm.ComponentModel;

namespace ExploreHttp.Models;
public partial class AuthenticationProvider : ObservableObject
{
    private AuthenticationKind kind;
    private BasicAuthenticationModel basic;
    private BearerAuthenticationModel bearer;
    private Oauth2AuthenticationModel oauth2;

    public AuthenticationKind Kind { get => kind; set => SetProperty(ref kind, value); }
    public BasicAuthenticationModel Basic { get => basic; set => SetProperty(ref basic, value); }
    public BearerAuthenticationModel Bearer { get => bearer; set => SetProperty(ref bearer, value); }
    public Oauth2AuthenticationModel Oauth2 { get => oauth2; set => SetProperty(ref oauth2, value); }

    public AuthenticationProvider()
    {
        Kind = AuthenticationKind.Inherit;
        Basic = new BasicAuthenticationModel();
        Bearer = new BearerAuthenticationModel();
        Oauth2 = new Oauth2AuthenticationModel();
    }
}

public enum AuthenticationKind
{
    Inherit,
    None,
    Basic,
    Bearer,
    Oauth2,
}
