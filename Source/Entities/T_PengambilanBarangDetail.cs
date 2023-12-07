using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VMS.Entities
{
    public class T_PengambilanBarangDetailDTO
    {
        public int ID { get; set; }
        public string? PBBNo { get; set; }
        public string? ItemID { get; set; }
        public string? ItemName { get; set; }
        public float? Qty { get; set; }
        public string? ExportedFileNo { get; set; }
        public DateTime? UploadDateTime { get; set; }
    }
}
