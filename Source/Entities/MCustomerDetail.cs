using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VMS.SharedKernel;

namespace VMS.Entities
{
    public class MCustomerDetail : BaseEntity
    {
        public string customerID { get; set; }
        public string customerName { get; set; }
        public string subCustomerID { get; set; }
        public string subCustomerName { get; set; }
    }

    public class MCustomerDetailExt
    {
        public long Id { get; set; }
        public string customerID { get; set; }
        public string customerName { get; set; }
        public string subCustomerID { get; set; }
        public string subCustomerName { get; set; }
    }

    public class MCustomerDetailDto
    {
        public long Id { get; set; }
        public string customerID { get; set; }
        public string customerName { get; set; }
        public string subCustomerID { get; set; }
        public string subCustomerName { get; set; }
        public string FlagData { get; set; }
    }
            
}
