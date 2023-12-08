using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VMS.Entities
{
    public class T_GRRequest
    {
        public T_GRRequest() 
        {
            DataDetail = new List<T_GRDetailRequest>();
        }

        public string? GRNo { get; set; }
        public DateTime GRDate { get; set; }
        public string? SupplierID { get; set; }
        public string? SupplierName { get; set; }
        public string? BranchID { get; set; }
        public string? BranchName { get; set; }
        public string? PONo { get; set; }
        public DateTime? PODate { get; set; }
        public string? RefNo { get; set; }
        public string? DeliveryBy { get; set; }
        public string? VehicleNo { get; set; }
        public string? ReceiveBy { get; set; }
        public string? Remark { get; set; }
        public float? TotalTransaction { get; set; }
        public float? DiscRate { get; set; }
        public float? DiscAmount { get; set; }
        public float? DiscAmount1 { get; set; }
        public float? TaxRate { get; set; }
        public float? TaxAmount { get; set; }
        public float? FinalTotal { get; set; }
        public int? StsActive { get; set; }
        public int? StsPReturn { get; set; }
        public int? StsPInv { get; set; }
        public int? StsPosted { get; set; }
        public string? EntryUser { get; set; }
        public string? SLocID { get; set; }
        public int? ReturnTypeId { get; set; }
        public string? Source { get; set; }
        public string? mode { get; set; }
        public string? userlogin { get; set; }
        public List<T_GRDetailRequest> DataDetail { get; set; }
    }
}
