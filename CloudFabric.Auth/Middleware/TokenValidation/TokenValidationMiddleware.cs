using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CloudFabric.Auth.Models;
using CloudFabric.Auth.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace CloudFabric.Auth.Middleware
{
    public class CloudFabricJwtValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public CloudFabricJwtValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IntrospectionHttpClientProvider httpClientProvider, IOptions<AuthClientOptions> authOptions)
        {
            var authorizationHeader = context.Request.Headers["Authorization"].ToString();

            if (authOptions == null || authOptions.Value == null || string.IsNullOrEmpty(authOptions.Value.Issuer))
            {
                throw new Exception("AuthOptions.Issuer is required to validate the token");
            }

            if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer"))
            {
                // Validation of the Token expiration
                var jwtEncodedString = authorizationHeader.Length > 10 ? authorizationHeader[7..] : "";

                var httpClient = httpClientProvider.GetHttpClient();
                var httpRequestMessage = new HttpRequestMessage(
                    HttpMethod.Post, $"{authOptions.Value.Issuer.TrimEnd('/')}/api/v1/auth/token_info"
                )
                {
                    Content = new FormUrlEncodedContent(
                        new[]
                        {
                            new KeyValuePair<string, string>("token", jwtEncodedString)
                        }
                    ),
                    Headers =
                    {
                        Authorization = new AuthenticationHeaderValue(
                            "Basic",
                            Convert.ToBase64String(Encoding.UTF8.GetBytes($"{authOptions.Value.ClientId}:{authOptions.Value.ClientSecret}"))
                        )
                    }
                };

                var response = await httpClient.SendAsync(httpRequestMessage);

                var responseString = await response.Content.ReadAsStringAsync();

                var tokenInfo = JsonSerializer.Deserialize<AccessTokenInfoViewModel>(responseString);
                if (!tokenInfo.IsActive)
                {
                    context.Response.Clear();
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync("Token is invalid");
                    return;
                }
            }

            await _next(context);
        }
    }
}