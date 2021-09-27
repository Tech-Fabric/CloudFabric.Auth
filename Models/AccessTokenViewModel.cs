using System;
using System.Text.Json.Serialization;

namespace Fiber.Auth.Models
{
    public class AccessTokenViewModel
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("expires_in")]
        public long ExpiresIn { get; set; }

        [JsonPropertyName("valid_to")]
        public DateTime ValidTo { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }
    }
}