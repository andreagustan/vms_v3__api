using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VMS.Entities
{
    public class T_ReturnList_BulkUpdate
    {
        public T_ReturnList_BulkUpdate() {
            DataDetail = new List<T_ReturnListDetail>();
        }
        public string? RLNo { get; set; }
        public DateTime? RLDate { get; set; }
        public DateTime? ExpDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string? GRNo { get; set; }
        public string? SupplierID { get; set; }
        public string? SupplierName { get; set; }
        public string? BranchID { get; set; }
        public string? BranchName { get; set; }
        public string? DelAddress { get; set; }
        public string? Remark { get; set; }
        public float? TotalTransaction { get; set; }
        public float? DiscRate { get; set; }
        public float? DiscAmount { get; set; }
        public float? TaxRate { get; set; }
        public float? TaxAmount { get; set; }
        public float? FinalTotal { get; set; }
        public int? StsActive { get; set; }
        public int? StsPosted { get; set; }
        public int? PostedUser { get; set; }
        public DateTime? PostedDate { get; set; }
        public string? ClosedUser { get; set; }
        public DateTime? ClosedDate { get; set; }
        public string? ExportedFileNo { get; set; }
        public DateTime? UploadDateTime { get; set; }
        public int? ApproveSupplier { get; set; }
        public DateTime? ApproveSupplierDateTime { get; set; }
        public string? EntryUser { get; set; }
        public bool JSONProcess { get; set; }
        [Required]
        public string mode { get; set; }
        public List<T_ReturnListDetail>  DataDetail { get; set; }
    }
}
