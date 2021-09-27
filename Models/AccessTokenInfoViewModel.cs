using System.Text.Json.Serialization;

namespace Fiber.Auth.Models
{
    public class AccessTokenInfoViewModel
    {
        [JsonPropertyName("active")]
        public bool IsActive { get; set; }

    }
}