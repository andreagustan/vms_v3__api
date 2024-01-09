using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VMS.Entities
{
    public class T_WasteDetail
    {
        public long? Id { get; set; }
        public string? WasteNo { get; set; }
        public string? ItemID { get; set; }
        public string? ItemName { get; set; }
        public string? UOM { get; set; }
        public float? COGS { get; set; }
        public float? Qty { get; set; }
        public float? SubTotal { get; set; }
        public string? SLocID { get; set; }
        public DateTime? ExpireDate { get; set; }
    }
}
