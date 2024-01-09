using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VMS.Entities
{
    public class T_ReturnListDetail
    {
        public long? Id { get; set; }
        public string? RLNo { get; set; }
        public string? ItemID { get; set; }
        public string? ItemName { get; set; }
        public string? UOM { get; set; }
        public float? Qty { get; set; }
        public float? Price { get; set; }
        public float? DiscRate1 { get; set; }
        public float? DiscAmount { get; set; }
        public float? SubTotal { get; set; }
        public long? IdPajak { get; set; }
        public float? Ppn { get; set; }
        public string? ItemIdHadiah { get; set; }
        public long? QtyIn { get; set; }
        public long? QtyHadiah { get; set; }
    }
}
