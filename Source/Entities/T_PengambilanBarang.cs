using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VMS.Entities
{
    public class T_PengambilanBarangRequest
    {
        public T_PengambilanBarangRequest() {
            DataDetail = new List<T_PengambilanBarangDetailDTO>();
        }
        public string PBBNo { get; set; }
        public DateTime? PBBDate { get; set; }
        public string? FromBranchID { get; set; }
        public string? FromBranchName { get; set; }
        public string? ToBranchID { get; set; }
        public string? ToBranchName { get; set; }
        public float? Total { get; set; }
        public string? Remark { get; set; }
        public int? StsPosted { get; set; }
        public string? TransNo { get; set; }
        public string? EntryUser { get; set; }
        public int? StsTransfer { get; set; }
        public DateTime? ExpireDate { get; set; }
        //public string? Source { get; set; }
        public bool JSONProcess { get; set; }
        [Required]
        public string mode { get; set; }
        public List<T_PengambilanBarangDetailDTO> DataDetail { get; set; }
    }
}
