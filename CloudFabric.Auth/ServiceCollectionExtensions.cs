using CloudFabric.Auth.Middleware;
using CloudFabric.Auth.Options;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Net.Http.Headers;

namespace CloudFabric.Auth
{
    public static class ServiceCollectionExtensions
    {
        private static readonly string JWT_OR_COOKIE_SCHEME = "JWT_OR_COOKIE";
        
        /// <summary>
        /// Adds token validation services, policies and handlers
        /// </summary>
        /// <param name="services"></param>
        /// <param name="authClientOptions"></param>
        /// <returns></returns>
        public static IServiceCollection AddCloudFabricAuth(
            this IServiceCollection services,
            IConfigurationSection authClientOptions
        )
        {
            services.AddHttpClient();
            services.Configure<AuthClientOptions>(authClientOptions);
            services.AddScoped<IntrospectionHttpClientProvider>();

            services.AddAuthentication(
                    options =>
                    {
                        options.DefaultAuthenticateScheme = JWT_OR_COOKIE_SCHEME;
                        options.DefaultChallengeScheme = JWT_OR_COOKIE_SCHEME;
                        options.DefaultScheme = JWT_OR_COOKIE_SCHEME;
                    }
                )
                .AddCookie(
                    options =>
                    {

                    }
                )
                .AddJwtBearer(
                    options =>
                    {
                        options.SaveToken = true;
                        options.RequireHttpsMetadata = false;

                        var jwtHandler = new JwtSecurityTokenHandler();

                        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

                        jwtHandler.InboundClaimTypeMap.Clear();
                        jwtHandler.OutboundClaimTypeMap.Clear();
                        //options.SecurityTokenValidators.Clear();
                        //options.SecurityTokenValidators.Add(jwtHandler);

                        options.TokenHandlers.Clear();
                        options.TokenHandlers.Add(jwtHandler);

                        var tokenOptions = authClientOptions.Get<AuthClientOptions>();
                        options.TokenValidationParameters =
                            new TokenValidationParameters()
                            {
                                ClockSkew = TimeSpan.Zero,
                                ValidateLifetime = true,
                                RequireExpirationTime = true,
                                ValidateIssuer = true,
                                ValidateAudience = true,
                                ValidAudience = tokenOptions.Audience,
                                ValidIssuer = tokenOptions.Issuer,
                                IssuerSigningKey = new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(tokenOptions.SymmetricSecurityKey)
                                )
                            };
                    }
                )
                .AddPolicyScheme(JWT_OR_COOKIE_SCHEME, JWT_OR_COOKIE_SCHEME, options =>
                {
                    // runs on each request
                    options.ForwardDefaultSelector = context =>
                    {
                        // filter by auth type
                        string authorization = context.Request.Headers[HeaderNames.Authorization];
                        if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
                            return JwtBearerDefaults.AuthenticationScheme;
                
                        // otherwise always check for cookie auth
                        return CookieAuthenticationDefaults.AuthenticationScheme;
                    };
                });

            //Register the Permission policy handlers
            services.AddSingleton<IAuthorizationMiddlewareResultHandler, PermissionResultHandler>();
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();

            return services;
        }
    }
}
