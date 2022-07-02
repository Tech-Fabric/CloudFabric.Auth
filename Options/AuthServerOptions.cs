using System.Collections.Generic;

namespace CloudFabric.Auth.Options
{
    public class AuthClient
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
    
    public class AuthServerOptions
    {
        public string SymmetricSecurityKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public double AccessTokenLifeTimeInSeconds { get; set; }

        public int RefreshTokenSize { get; set; }
        public double RefreshTokenLifetimeInSeconds { get; set; }

        public List<AuthClient> Clients { get; set; } = new List<AuthClient>();
    }
}