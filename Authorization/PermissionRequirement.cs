using Microsoft.AspNetCore.Authorization;

namespace Fiber.Auth
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public PermissionRequirement(int permissionId)
        {
            PermissionId = permissionId;
        }

        public int PermissionId { get; }
    }
}
