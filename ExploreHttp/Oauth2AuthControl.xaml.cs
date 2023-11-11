using ExploreHttp.Models;
using System.Windows;
using System.Windows.Controls;

namespace ExploreHttp;
/// <summary>
/// Interaction logic for Oauth2AuthControl.xaml
/// </summary>
public partial class Oauth2AuthControl : UserControl
{
    public Oauth2AuthControl()
    {
        InitializeComponent();
    }

    private static DependencyProperty GrantTypeProperty = DependencyProperty.Register(
        nameof(GrantType),
        typeof(string),
        typeof(Oauth2AuthControl));
    private static DependencyProperty ClientIdProperty = DependencyProperty.Register(
        nameof(ClientId),
        typeof(string),
        typeof(Oauth2AuthControl));
    private static DependencyProperty ClientSecretProperty = DependencyProperty.Register(
        nameof(ClientSecret),
        typeof(string),
        typeof(Oauth2AuthControl));
    private static DependencyProperty UsernameProperty = DependencyProperty.Register(
        nameof(Username),
        typeof(string),
        typeof(Oauth2AuthControl));
    private static DependencyProperty PasswordProperty = DependencyProperty.Register(
        nameof(Password),
        typeof(string),
        typeof(Oauth2AuthControl));
    private static DependencyProperty ScopeProperty = DependencyProperty.Register(
        nameof(Scope),
        typeof(string),
        typeof(Oauth2AuthControl));
    private static DependencyProperty AudienceProperty = DependencyProperty.Register(
        nameof(Audience),
        typeof(string),
        typeof(Oauth2AuthControl));

    public string GrantType
    {
        get => (string)GetValue(GrantTypeProperty);
        set => SetValue(GrantTypeProperty, value);
    }
    public string ClientId
    {
        get => (string)GetValue(ClientIdProperty);
        set => SetValue(ClientIdProperty, value);
    }
    public string ClientSecret
    {
        get => (string)GetValue(ClientSecretProperty);
        set => SetValue(ClientSecretProperty, value);
    }
    public string Username
    {
        get => (string)GetValue(UsernameProperty);
        set => SetValue(UsernameProperty, value);
    }
    public string Password
    {
        get => (string)GetValue(PasswordProperty);
        set => SetValue(PasswordProperty, value);
    }
    public string Scope
    {
        get => (string)GetValue(ScopeProperty);
        set => SetValue(ScopeProperty, value);
    }
    public string Audience
    {
        get => (string)GetValue(AudienceProperty);
        set => SetValue(AudienceProperty, value);
    }
}
