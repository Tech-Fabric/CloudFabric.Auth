namespace CloudFabric.Auth.Models.Request
{
    public class TokenInfoRequest
    {
        /// <summary>
        /// 'access_token' or 'refresh_token'
        /// </summary>
        [JsonPropertyName("token_type_hint")]
        public string TokenTypeHint { get; set; }
        
        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}