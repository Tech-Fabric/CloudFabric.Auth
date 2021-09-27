using Fiber.Auth.Enums;
using Fiber.Auth.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fiber.Auth
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        public PermissionHandler(
        )
        {
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement
        )
        {
            var userId = context.User.Identity.GetUserId();

            var accessControlGroups = context.User.Identity.GetUserAccessControlGroups();

            var globalAccessControlGroups = accessControlGroups
                .FirstOrDefault(g => g.TenantId == Guid.Empty);

            if (globalAccessControlGroups == null)
            {
                throw new Exception("User does not have default global AccessControlGroup");
            }

            AccessControlGroupViewModel tenantAccessControl = null;

            if (context.Resource is DefaultHttpContext httpContextForTenantId)
            {
                if (httpContextForTenantId.Request.RouteValues.ContainsKey("tenantId"))
                {
                    var pathTenantId = httpContextForTenantId?.Request?.RouteValues["tenantId"]?.ToString();

                    if (!string.IsNullOrEmpty(pathTenantId))
                    {
                        var tenantId = Guid.Parse(pathTenantId);

                        tenantAccessControl = accessControlGroups
                            .FirstOrDefault(g => g.TenantId == tenantId);

                        if (tenantAccessControl == null)
                        {
                            context.Fail();
                            return Task.CompletedTask;
                        }
                    }
                }
            }

            if (tenantAccessControl != null)
            {
                var tenantResult = ValidateAccessControlGroup(requirement.PermissionId, tenantAccessControl);

                if (tenantResult == AccessControlRuleEnum.Allow)
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
                
                if (tenantResult == AccessControlRuleEnum.Deny)
                {
                    context.Fail();
                    return Task.CompletedTask;
                }
            }

            var globalAccessControlResult = ValidateAccessControlGroup(requirement.PermissionId, globalAccessControlGroups);
            if (globalAccessControlResult == AccessControlRuleEnum.Deny)
            {
                context.Fail();
                return Task.CompletedTask;
            }
            
            if (globalAccessControlResult == AccessControlRuleEnum.Allow)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            context.Fail();
            return Task.CompletedTask;
        }

        public static AccessControlRuleEnum ValidatePermissions(
            int permission,
            List<AccessControlSettingViewModel> accessControlSettings
        )
        {
            // TODO: change to accumulate all results in a list and then check if there is at least one deny
            foreach (var accessControlSetting in accessControlSettings)
            {
                if (accessControlSetting.PermissionId == permission)
                {
                    if (accessControlSetting.Rule == AccessControlRuleEnum.Deny)
                    {
                        return AccessControlRuleEnum.Deny;
                    }
                    else if (accessControlSetting.Rule == AccessControlRuleEnum.Allow)
                    {
                        return AccessControlRuleEnum.Allow;
                    }
                }
            }

            return AccessControlRuleEnum.NotSet;
        }

        public static AccessControlRuleEnum ValidateAccessControlGroup(
            int permission,
            AccessControlGroupViewModel accessControlGroup
        )
        {
            var permissionsResult = ValidatePermissions(permission, accessControlGroup.Rules);

            if (permissionsResult != AccessControlRuleEnum.NotSet)
            {
                return permissionsResult;
            }

            return AccessControlRuleEnum.NotSet;
        }

        public static AccessControlRuleEnum ValidateAccess(
            int permission,
            AccessControlViewModel accessControl
        )
        {
            if (accessControl.Rules != null)
            {
                var permissionsResult = ValidatePermissions(permission, accessControl.Rules);

                if (permissionsResult != AccessControlRuleEnum.NotSet)
                {
                    return permissionsResult;
                }
            }

            return AccessControlRuleEnum.NotSet;
        }
    }
}