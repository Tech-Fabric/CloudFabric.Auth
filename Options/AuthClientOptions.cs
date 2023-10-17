namespace CloudFabric.Auth.Options
{
    public class AuthClientOptions
    {
        public string SymmetricSecurityKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public int Demo { get; set; }
    }
}