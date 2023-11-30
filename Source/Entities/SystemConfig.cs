using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VMS.SharedKernel;

namespace VMS.Entities
{
    public class SystemConfig: BaseEntity
    {
        public string Name { get; set; }
        public string Result { get; set; }
        public string SystemCategory { get; set; }
        public string SystemSubCategory { get; set; }
        public string SystemCode { get; set; }
        public string SystemValue { get; set; }
        public string Description { get; set; }
    }

    public class SystemConfigParm 
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string SystemCategory { get; set; }
        public string SystemSubCategory { get; set; }
        public string SystemCode { get; set; }
        public string SystemValue { get; set; }
        public string Description { get; set; }
        public string FlagData { get; set; }
    }

    public class SystemConfigExt
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string SystemCategory { get; set; }
        public string SystemSubCategory { get; set; }
        public string SystemCode { get; set; }
        public string SystemValue { get; set; }
        public string Description { get; set; }
    }
}
