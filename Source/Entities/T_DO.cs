using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VMS.Entities
{
    public class T_DORequest
    {
        public T_DORequest() 
        {
            DataDetail = new List<T_DODetailRequest>();
        }
        public string DONo { get; set; }
        public DateTime? DODate { get; set; }
        public string? CustomerID { get; set; }
        public string? CustomerName { get; set; }
        public string? BranchID { get; set; }
        public string? BranchName { get; set; }
        public string? SONo { get; set; }
        public DateTime? SODate { get; set; }
        public string? RefNo { get; set; }
        public string? DeliveryBy { get; set; }
        public string? VehicleNo { get; set; }
        public string? Remark { get; set; }
        public float? TotalTransaction { get; set; }
        public float? DiscRate { get; set; }
        public float? DiscAmount { get; set; }
        public float? DiscAmount1 { get; set; }
        public float? TaxRate { get; set; }
        public float? TaxAmount { get; set; }
        public float? FinalTotal { get; set; }
        public int? StsActive { get; set; }
        public int? StsSReturn { get; set; }
        public int? StsSInv { get; set; }
        public int? StsPosted { get; set; }
        public string? EntryUser { get; set; }
        public string? SLocID { get; set; }
        public DateTime? perioddate { get; set; }
        public int? SetoranTunaiId { get; set; }
        public string? Source { get; set; }
        public List<T_DODetailRequest> DataDetail { get; set; }
    }
}
