using Fiber.Auth.Middleware;
using Fiber.Auth.Options;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Fiber.Auth
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds token validation services, policies and handlers
        /// </summary>
        /// <param name="services"></param>
        /// <param name="authClientOptions"></param>
        /// <returns></returns>
        public static IServiceCollection AddFiberAuth(
            this IServiceCollection services,
            IConfigurationSection authClientOptions
        )
        {
            services.Configure<AuthClientOptions>(authClientOptions);
            services.AddScoped<IntrospectionHttpClientProvider>();

            services.AddAuthentication(
                    options =>
                    {
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    }
                )
                .AddJwtBearer(
                    options =>
                    {
                        options.SaveToken = true;
                        options.RequireHttpsMetadata = false;

                        var jwtHandler = new JwtSecurityTokenHandler();
                        jwtHandler.InboundClaimTypeMap.Clear();
                        options.SecurityTokenValidators.Clear();
                        options.SecurityTokenValidators.Add(jwtHandler);

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
                );

            //Register the Permission policy handlers
            services.AddSingleton<IAuthorizationMiddlewareResultHandler, PermissionResultHandler>();
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();

            return services;
        }
    }
}
