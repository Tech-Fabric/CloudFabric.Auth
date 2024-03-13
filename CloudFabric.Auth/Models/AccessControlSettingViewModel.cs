using CloudFabric.Auth.Enums;

namespace CloudFabric.Auth.Models
{
    public class AccessControlSettingViewModel
    {
        public int PermissionId { get; set; }

        public AccessControlRuleEnum Rule { get; set; }
    }
}
