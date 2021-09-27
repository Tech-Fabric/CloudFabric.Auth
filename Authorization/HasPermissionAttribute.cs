using Microsoft.AspNetCore.Authorization;

using System;

namespace Fiber.Auth
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false)]
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(int permission) : base($"HasPermission{permission}")
        { }
    }
}
