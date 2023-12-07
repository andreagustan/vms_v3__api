using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VMS.Entities
{
    public class T_PermintaanBarangDetailRequest
    {
        public int? ID { get; set; }
        public string PBNo { get; set; }
        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public float? Qty { get; set; }
        public float? SOH { get; set; }
        public float? AvgDay { get; set; }
        public float? LeadTime { get; set; }
        public float? SCD { get; set; }
        public int? MaxStore { get; set; }
    }
}
