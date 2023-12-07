using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VMS.Entities
{
    public class T_GRDetailRequest
    {
        public int? ID { get; set; }
        public string? GRNo { get; set; }
        public string? ItemID { get; set; }
        public string? ItemName { get; set; }
        public string? UOM { get; set; }
        public float? Qty { get; set; }
        public float? Price { get; set; }
        public float? DiscRate1 { get; set; }
        public float? DiscRate2 { get; set; }
        public float? DiscAmount { get; set; }
        public float? SubTotal { get; set; }
        public string? SLocID { get; set; }
        public string? ExpireDate { get; set; }
        public int? ppn { get; set; }

    }
}
