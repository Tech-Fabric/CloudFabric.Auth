using System.Text.Json.Serialization;

namespace CloudFabric.Auth.Models
{
    public class AccessTokenInfoViewModel
    {
        [JsonPropertyName("active")]
        public bool IsActive { get; set; }

    }
}