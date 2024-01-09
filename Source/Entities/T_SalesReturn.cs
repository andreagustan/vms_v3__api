using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VMS.Entities
{
    public class T_SalesReturn_BulkUpdate
    {
        public T_SalesReturn_BulkUpdate() {
            DataDetail = new List<T_SalesReturnDetail>();
        }
        public string? SRetNo { get; set; }
        public DateTime? SRetDate { get; set; }
        public string? CustomerID { get; set; }
        public string? CustomerName { get; set; }
        public string? BranchID { get; set; }
        public string? BranchName { get; set; }
        public string? DONo { get; set; }
        public DateTime? DODate { get; set; }
        public float? TotalTransaction { get; set; }
        public float? DiscRate { get; set; }
        public float? DiscAmount { get; set; }
        public float? DiscAmount1 { get; set; }
        public float? TaxRate { get; set; }
        public float? TaxAmount { get; set; }
        public float? FinalAmount { get; set; }
        public string? Remark { get; set; }
        public long? StsActive { get; set; }
        public long? StsSInv { get; set; }
        public long? StsPosted { get; set; }
        public string? PostedUser { get; set; }
        public DateTime? PostedDate { get; set; }
        public string? ExportedFileNo { get; set; }
        public DateTime? UploadDateTime { get; set; }
        public string? SLocID { get; set; }
        public string? EntryUser { get; set; }
        public bool JSONProcess { get; set; }
        [Required]
        public string mode { get; set; }
        public List<T_SalesReturnDetail> DataDetail { get; set; }
    }
}
