using Microsoft.AspNetCore.Builder;

namespace Fiber.Auth.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseFiberJwtValidationMiddleware(
            this IApplicationBuilder builder
        )
        {
            return builder.UseMiddleware<FiberJwtValidationMiddleware>();
        }
    }
}