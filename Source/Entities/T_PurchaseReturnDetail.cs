using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VMS.Entities
{
    public class T_PurchaseReturnDetail
    {
        public long? Id { get; set; }
        public string? PRetNo { get; set; }
        public string? ItemID { get; set; }
        public string? ItemName { get; set; }
        public float? Qty { get; set; }
        public string? UOM { get; set; }
        public float? Price { get; set; }
        public float? DiscRate { get; set; }
        public float? DiscAmount { get; set; }
        public float? DetailAmount { get; set; }
        public string? SLocID { get; set; }
        public DateTime? ExpireDate { get; set; }
        public long? DiscRate1 { get; set; }
        public long? DiscRate2 { get; set; }
        public long? ppn { get; set; }
    }
}
