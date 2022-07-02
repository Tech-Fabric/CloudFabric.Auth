using CloudFabric.Auth.Enums;
using CloudFabric.Auth.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CloudFabric.Auth
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly ILogger _logger;
        
        public PermissionHandler(
            ILogger<PermissionHandler> logger
        )
        {
            _logger = logger;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement
        )
        {
            var userId = context.User.Identity.GetUserId();
            
            _logger.LogTrace("Verifying permissions for user {UserId}", userId);

            var accessControlGroups = context.User.Identity.GetUserAccessControlGroups();

            var globalAccessControlGroups = accessControlGroups
                .FirstOrDefault(g => g.TenantId == Guid.Empty);

            if (globalAccessControlGroups == null)
            {
                throw new Exception("User does not have default global AccessControlGroup");
            }

            List<AccessControlGroupViewModel> tenantAccessControlGroups = null;

            if (context.Resource is DefaultHttpContext httpContextForTenantId)
            {
                if (httpContextForTenantId.Request.RouteValues.ContainsKey("tenantId"))
                {
                    var pathTenantId = httpContextForTenantId?.Request?.RouteValues["tenantId"]?.ToString();

                    if (!string.IsNullOrEmpty(pathTenantId))
                    {
                        var tenantId = Guid.Parse(pathTenantId);

                        tenantAccessControlGroups = accessControlGroups
                            .Where(g => g.TenantId == tenantId).ToList();

                        if (tenantAccessControlGroups.Count <= 0)
                        {
                            _logger.LogTrace("Verifying permissions for user {UserId}: " +
                                             "found tenantId {TenantId} in routeValues but didn't found any " +
                                             "tenant access control groups in claims", userId, tenantId);
                            
                            context.Fail();
                            return Task.CompletedTask;
                        }
                    }
                }
            }

            if (tenantAccessControlGroups != null)
            {
                var results = new List<AccessControlRuleEnum>();
                
                foreach (var tenantAccessControlGroup in tenantAccessControlGroups)
                {
                    results.Add(ValidateAccessControlGroup(requirement.PermissionId, tenantAccessControlGroup));
                }
                
                if(results.Any(r => r == AccessControlRuleEnum.Deny))
                {
                    _logger.LogTrace("Denying because one of tenant groups has Deny setting");
                    context.Fail();
                    return Task.CompletedTask;
                }
                
                if(results.Any(r => r == AccessControlRuleEnum.Allow))
                {
                    _logger.LogTrace("Allowing because any tenant groups have Deny setting and some of tenant groups have Allow setting");
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }

            var globalAccessControlResult = ValidateAccessControlGroup(requirement.PermissionId, globalAccessControlGroups);
            if (globalAccessControlResult == AccessControlRuleEnum.Deny)
            {
                _logger.LogTrace("Denying because global group has Deny setting");
                context.Fail();
                return Task.CompletedTask;
            }
            
            if (globalAccessControlResult == AccessControlRuleEnum.Allow)
            {
                _logger.LogTrace("Allowing because global group has Allow setting");
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            _logger.LogTrace("Failing because there were no Allow settings found");
            
            context.Fail();
            return Task.CompletedTask;
        }

        public static AccessControlRuleEnum ValidatePermissions(
            int permission,
            List<AccessControlSettingViewModel> accessControlSettings
        )
        {
            var results = new List<AccessControlRuleEnum>();
            
            foreach (var accessControlSetting in accessControlSettings)
            {
                if (accessControlSetting.PermissionId == permission)
                {
                    results.Add(accessControlSetting.Rule);
                }
            }

            if(results.Any(r => r == AccessControlRuleEnum.Deny))
            {
                return AccessControlRuleEnum.Deny;
            }
            
            if(results.Any(r => r == AccessControlRuleEnum.Allow))
            {
                return AccessControlRuleEnum.Allow;
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