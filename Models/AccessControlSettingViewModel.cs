using Fiber.Auth.Enums;

namespace Fiber.Auth.Models
{
    public class AccessControlSettingViewModel
    {
        public int PermissionId { get; set; }

        public AccessControlRuleEnum Rule { get; set; }
    }
}
