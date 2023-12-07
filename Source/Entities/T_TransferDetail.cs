using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VMS.Entities
{
    public class T_TransferDetailDTO
    {
        public int? ID { get; set; }
        public string? TransNo { get; set; }
        public string? ItemID { get; set; }
        public string? ItemName { get; set; }
        public string? UOM { get; set; }
        public float? Qty { get; set; }
        public float? COGS { get; set; }
        public float? SubTotal { get; set; }
        public string? ExportedFileNo { get; set; }
        public DateTime? UploadDateTime { get; set; }
        public string? ExportedFileNo2 { get; set; }
        public string? SLocIDKirim { get; set; }
        public string? SLocIDTerima { get; set; }
        public DateTime? ExpireDate { get; set; }
    }
}
