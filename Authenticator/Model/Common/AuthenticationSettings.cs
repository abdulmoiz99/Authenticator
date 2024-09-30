namespace Authenticator.Model.Common
{
    public class AuthenticationSettings
    {
        public GoogleAuthSettings Google { get; set; }
        public GitHubAuthSettings GitHub { get; set; }
    }

    public class GoogleAuthSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }

    public class GitHubAuthSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }

}
