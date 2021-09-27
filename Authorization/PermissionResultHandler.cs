using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;

namespace Fiber.Auth
{
    public class PermissionResultHandler : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler
            DefaultHandler = new AuthorizationMiddlewareResultHandler();

        public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
        {
            //return policyAuthorizationResult.Forbidden &&
            //policyAuthorizationResult.AuthorizationFailure.FailedRequirements.OfType<
            //                                               Show404Requirement>().Any();

            // Fallback to the default implementation.
            await DefaultHandler.HandleAsync(next, context, policy, authorizeResult);
            //context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            return;
        }
    }
}
