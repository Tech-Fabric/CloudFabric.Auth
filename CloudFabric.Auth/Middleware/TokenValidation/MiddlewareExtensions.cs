using Microsoft.AspNetCore.Builder;

namespace CloudFabric.Auth.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseCloudFabricJwtValidationMiddleware(
            this IApplicationBuilder builder
        )
        {
            return builder.UseMiddleware<CloudFabricJwtValidationMiddleware>();
        }
    }
}