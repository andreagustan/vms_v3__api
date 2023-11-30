using VMS.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSO.Entities
{
    public partial class MasterApplications : BaseEntity
    {
        public string AppId { get; set; }
        public string AppName { get; set; }
        public string AppDesc { get; set; }
        public string AppType { get; set; }
        public string AppAddress { get; set; }
        public string ActiveYN { get; set; }
    }
}
