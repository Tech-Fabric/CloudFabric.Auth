using System;
using System.Collections.Generic;
using Fiber.Auth.Models;

namespace Fiber.Auth
{
    public class ClaimsHelper
    {
        public static string GetPermissionsClaim(List<AccessControlGroupViewModel> userAccessControlGroups)
        {
            var groups = new List<string>();
            foreach (var g in userAccessControlGroups)
            {
                groups.Add(GetPermissionsClaimStringForGroup(g));
            }

            return string.Join("|", groups);
        }
        
        
        private static string GetPermissionsClaimStringForGroup(AccessControlGroupViewModel group)
        {
            var rules = new List<string>();
            foreach (var rule in group.Rules)
            {
                rules.Add($"{(int)rule.PermissionId}:{(int)rule.Rule}");
            }

            var tenantIdSerialized = group.TenantId == Guid.Empty ? "global" : group.TenantId.ToString();
            
            return $"{tenantIdSerialized}#{group.Id}#{group.MachineName}#{string.Join(",", rules)}";
        }
    }
}