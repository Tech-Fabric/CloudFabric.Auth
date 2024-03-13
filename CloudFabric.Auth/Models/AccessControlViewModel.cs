using System;
using System.Collections.Generic;

namespace CloudFabric.Auth.Models
{
    public class AccessControlViewModel
    {
        public Guid TenantId { get; set; }

        public List<AccessControlSettingViewModel> Rules { get; set; }
    }
}
