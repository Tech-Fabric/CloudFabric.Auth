using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

using System.Threading.Tasks;

namespace CloudFabric.Auth
{
    public class PermissionAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        private readonly AuthorizationOptions _options;

        public PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {
            _options = options.Value;
        }

        public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            return await base.GetPolicyAsync(policyName)
                   ?? new AuthorizationPolicyBuilder()
                       .AddRequirements(new PermissionRequirement(int.Parse(policyName.Replace("HasPermission", ""))))
                       .Build();
        }
    }
}
