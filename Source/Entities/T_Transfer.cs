using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VMS.Entities
{
    public class T_TransferRequest
    {
        public T_TransferRequest() 
        {
            DataDetail = new List<T_TransferDetailDTO>();
        }
        public string? TransNo { get; set; }
        public DateTime? TransDate { get; set; }
        public string? BranchIDKirim { get; set; }
        public string? BranchNameKirim { get; set; }
        public string? DeliveryBy { get; set; }
        public string? VehicleNo { get; set; }
        public float? Amount { get; set; }
        public string? Remark { get; set; }
        public DateTime? TransTerimaDate { get; set; }
        public string? BranchIDTerima { get; set; }
        public string? BranchNameTerima { get; set; }
        public string? ReceiveBy { get; set; }
        public string? EntryUser { get; set; }
        public int? StsPosted { get; set; }
        public int? StsApprove { get; set; }
        public int? StsCancel { get; set; }
        public string? PBNo { get; set; }
        public string? SLocID { get; set; }
        public string? JenisKirim { get; set; }
        //public string? Source { get; set; }
        public bool JSONProcess { get; set; }
        [Required]
        public string mode { get; set; }
        public List<T_TransferDetailDTO> DataDetail { get; set; }
    }
}
