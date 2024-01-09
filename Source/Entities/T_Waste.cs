using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VMS.Entities
{
    public class T_Waste_BulkUpdate
    {
        public T_Waste_BulkUpdate() {
            DataDetail = new List<T_WasteDetail>();
        }
        public string? WasteNo { get; set; }
        public DateTime? WasteDate { get; set; }
        public string? BranchID { get; set; }
        public string? BranchName { get; set; }
        public float? Total { get; set; }
        public string? Remark { get; set; }
        public int? StsPosted { get; set; }
        public string? PostedUser { get; set; }
        public DateTime? PostedDate { get; set; }
        public string? ExportedFileNo { get; set; }
        public DateTime? UploadDateTime { get; set; }
        public string? EntryUser { get; set; }
        public int? StsApprove { get; set; }
        public string? ApproveUser { get; set; }
        public DateTime? ApproveDate { get; set; }
        public string? SLocID { get; set; }
        public bool JSONProcess { get; set; }
        [Required]
        public string mode { get; set; }
        public List<T_WasteDetail> DataDetail { get; set; }
    }
}
