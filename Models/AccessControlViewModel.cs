using System;
using System.Collections.Generic;

namespace Fiber.Auth.Models
{
    public class AccessControlViewModel
    {
        public Guid TenantId { get; set; }

        public List<AccessControlSettingViewModel> Rules { get; set; }
    }
}
