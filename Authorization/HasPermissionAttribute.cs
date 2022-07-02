using Microsoft.AspNetCore.Authorization;

using System;

namespace CloudFabric.Auth
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false)]
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(int permission) : base($"HasPermission{permission}")
        { }
    }
}
