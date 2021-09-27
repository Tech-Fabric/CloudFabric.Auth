
using System;
using System.Collections.Generic;

namespace Fiber.Auth.Models
{
    public class AccessControlGroupViewModel
    {
        public Guid Id { get; set; }

        public string MachineName { get; set; }
        
        public Guid TenantId { get; set; }
        
        public List<AccessControlSettingViewModel> Rules { get; set; }

        public bool IsUserPersonalGroup { get; set; }

        public List<Guid> UsersIds { get; set; }
    }
}
