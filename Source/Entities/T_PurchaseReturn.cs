using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VMS.Entities
{
    public class T_PurchaseReturn_BulkUpdate
    {
        public T_PurchaseReturn_BulkUpdate() {
            DataDetail = new List<T_PurchaseReturnDetail>();
        }
        public string? PRetNo { get; set; }
        public DateTime? PRetDate { get; set; }
        public string? SupplierID { get; set; }
        public string? SupplierName { get; set; }
        public string? BranchID { get; set; }
        public string? BranchName { get; set; }
        public string? GRNo { get; set; }
        public DateTime? GRDate { get; set; }
        public float? TotalTransaction { get; set; }
        public float? DiscRate { get; set; }
        public float? DiscAmount { get; set; }
        public float? DiscAmount1 { get; set; }
        public float? TaxRate { get; set; }
        public float? TaxAmount { get; set; }
        public float? FinalAmount { get; set; }
        public string? Remark { get; set; }
        public long? StsActive { get; set; }
        public long? StsPInv { get; set; }
        public long? StsPosted { get; set; }
        public string? PostedUser { get; set; }
        public DateTime? PostedDate { get; set; }
        public string? ExportedFileNo { get; set; }
        public DateTime? UploadDateTime { get; set; }
        public string? SLocID { get; set; }
        public long? NRId { get; set; }
        public long? ReturnTypeID { get; set; }
        public DateTime? RLDate { get; set; }
        public string? RLNo { get; set; }
        public DateTime? PeriodDate { get; set; }
        public string? EntryUser { get; set; }
        public bool JSONProcess { get; set; }
        [Required]
        public string mode { get; set; }
        public List<T_PurchaseReturnDetail> DataDetail { get; set; }
    }
}
