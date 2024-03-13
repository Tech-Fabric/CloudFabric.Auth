using CloudFabric.Auth.Enums;
using CloudFabric.Auth.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace CloudFabric.Auth
{
    public static class PrincipalExtensions
    {
        public static Guid GetUserId(this IIdentity identity)
        {
            var id = identity as ClaimsIdentity;
            var claim = id?.FindFirst("sub");

            if (claim == null)
            {
                throw new InvalidOperationException(
                    "Token is invalid",
                    new Exception("sub claim is missing")
                );
            }

            return Guid.Parse(claim.Value);
        }

        public static List<AccessControlGroupViewModel> GetUserAccessControlGroups(this IIdentity identity)
        {
            var id = identity as ClaimsIdentity;
            var claim = id?.FindFirst("p");

            if (claim == null)
            {
                throw new InvalidOperationException(
                    "Token is invalid",
                    new Exception("p claim is missing")
                );
            }

            var accessControlGroups = new List<AccessControlGroupViewModel>();

            foreach (var g in claim.Value.Split("|"))
            {
                var groupDetails = g.Split("#");

                var accessControlGroup = new AccessControlGroupViewModel()
                {
                    TenantId = groupDetails[0] == "global" ? Guid.Empty : Guid.Parse(groupDetails[0]),
                    Id = Guid.Parse(groupDetails[1]),
                    MachineName = groupDetails[2],
                    Rules = groupDetails[3].Split(",").Select(r => new AccessControlSettingViewModel()
                    {
                        PermissionId = int.Parse(r.Split(":")[0]),
                        Rule = Enum.Parse<AccessControlRuleEnum>(r.Split(":")[1]),
                    }).ToList()
                };

                accessControlGroups.Add(accessControlGroup);
            }

            return accessControlGroups;
        }
    }
}