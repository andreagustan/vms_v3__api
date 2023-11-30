using VMS.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VMS.Entities
{
    public partial class MasterAppsModul: BaseEntity
    {
        public string ModId { get; set; }
        public string ModApps { get; set; }
        public string ModName { get; set; }
        public string ModDesc { get; set; }
        public string ActiveYN { get; set; }
    }
}
