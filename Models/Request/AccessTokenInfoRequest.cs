namespace Fiber.Auth.Models.Request
{
    public class TokenInfoRequest
    {
        /// <summary>
        /// 'access_token' or 'refresh_token'
        /// </summary>
        public string token_type_hint { get; set; }
        
        public string token { get; set; }
    }
}