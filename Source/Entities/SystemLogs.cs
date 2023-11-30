using VMS.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VMS.Entities
{
    public class SystemLogsList : BaseEntity
    {
        public string Description { get; set; }
        public string Module { get; set; }
        public string Action { get; set; }
        public string StatusLog { get; set; }
    }

    public class SystemLogsDetail
    {
        public long Id { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public string Note1 { get; set; }
        public string ErrorLog { get; set; }
    }
}
